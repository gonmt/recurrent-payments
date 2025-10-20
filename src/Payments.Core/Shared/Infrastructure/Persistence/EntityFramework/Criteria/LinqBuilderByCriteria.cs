using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Payments.Core.Shared.Domain.FiltersByCriteria;
using Payments.Core.Shared.Domain.ValueObjects;

using DomainCriteria = Payments.Core.Shared.Domain.FiltersByCriteria.Criteria;

namespace Payments.Core.Shared.Infrastructure.Persistence.EntityFramework.Criteria;

public static class LinqBuilderByCriteria
{
    public static IQueryable<T> Where<T>(this IQueryable<T> source, DomainCriteria criteria)
    {
        if (!criteria.HasFilters())
        {
            return source;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "entity");
        Expression? accumulated = null;

        foreach (Filter filter in criteria.Filters!.Values)
        {
            Expression? filterExpression = BuildFilterExpression(parameter, filter);

            if (filterExpression is null)
            {
                continue;
            }

            accumulated = accumulated is null
                ? filterExpression
                : Expression.AndAlso(accumulated, filterExpression);
        }

        if (accumulated is null)
        {
            return source;
        }

        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(accumulated, parameter);
        return source.Where(lambda);
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, DomainCriteria criteria)
    {
        if (!criteria.HasOrder())
        {
            return source;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "entity");
        Expression keyAccess = BuildMemberAccess(parameter, criteria.Order!.OrderBy.Value);
        LambdaExpression keySelector = Expression.Lambda(keyAccess, parameter);

        return criteria.Order.OrderType switch
        {
            OrderType.ASC => ApplyOrdering(source, nameof(Queryable.OrderBy), keySelector, keyAccess.Type),
            OrderType.DESC => ApplyOrdering(source, nameof(Queryable.OrderByDescending), keySelector, keyAccess.Type),
            _ => source
        };
    }

    public static IQueryable<T> Limit<T>(this IQueryable<T> source, DomainCriteria criteria)
    {
        if (criteria.Limit is null or 0)
        {
            return source;
        }

        return source.Take(criteria.Limit.Value);
    }

    public static IQueryable<T> Offset<T>(this IQueryable<T> source, DomainCriteria criteria)
    {
        if (criteria.Offset is null or 0)
        {
            return source;
        }

        return source.Skip(criteria.Offset.Value);
    }

    private static Expression? BuildFilterExpression(ParameterExpression parameter, Filter filter)
    {
        Expression member = BuildMemberAccess(parameter, filter.Field.Value);
        Type memberType = member.Type;
        Type targetType = Nullable.GetUnderlyingType(memberType) ?? memberType;

        return filter.Operator switch
        {
            FilterOperator.EQUAL => BuildComparison(member, filter.Value.Value, targetType, Expression.Equal),
            FilterOperator.NOTEQUAL => BuildComparison(member, filter.Value.Value, targetType, Expression.NotEqual),
            FilterOperator.GT => BuildComparison(member, filter.Value.Value, targetType, Expression.GreaterThan),
            FilterOperator.GTE => BuildComparison(member, filter.Value.Value, targetType, Expression.GreaterThanOrEqual),
            FilterOperator.LT => BuildComparison(member, filter.Value.Value, targetType, Expression.LessThan),
            FilterOperator.LTE => BuildComparison(member, filter.Value.Value, targetType, Expression.LessThanOrEqual),
            FilterOperator.CONTAINS => BuildContains(member, filter.Value.Value, positive: true),
            FilterOperator.NOTCONTAINS => BuildContains(member, filter.Value.Value, positive: false),
            _ => null
        };
    }

    private static BinaryExpression? BuildComparison(
        Expression member,
        string rawValue,
        Type targetType,
        Func<Expression, Expression, BinaryExpression> comparer)
    {
        object? converted = ConvertToType(rawValue, targetType);

        if (converted is null)
        {
            return null;
        }

        Expression constant = Expression.Constant(converted, targetType);

        if (member.Type != targetType)
        {
            member = Expression.Convert(member, targetType);
        }

        return comparer(member, constant);
    }

    private static Expression? BuildContains(Expression member, string rawValue, bool positive)
    {
        Expression? stringMember = GetStringAccessor(member);

        if (stringMember is null)
        {
            return null;
        }

        MethodInfo contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
        Expression call = Expression.Call(stringMember, contains, Expression.Constant(rawValue, typeof(string)));
        return positive ? call : Expression.Not(call);
    }

    private static Expression? GetStringAccessor(Expression member)
    {
        if (member.Type == typeof(string))
        {
            return member;
        }

        PropertyInfo? valueProperty = member.Type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

        if (valueProperty?.PropertyType == typeof(string))
        {
            return Expression.Property(member, valueProperty);
        }

        MethodInfo? toString = member.Type.GetMethod(nameof(ToString), Type.EmptyTypes);
        return toString is null ? null : Expression.Call(member, toString);
    }

    private static Expression BuildMemberAccess(Expression parameter, string path)
    {
        string[] parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        Expression current = parameter;

        foreach (string part in parts)
        {
            current = Expression.PropertyOrField(current, part);
        }

        return current;
    }

    private static object? ConvertToType(string rawValue, Type targetType)
    {
        Type effectiveType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (effectiveType == typeof(string))
        {
            return rawValue;
        }

        if (typeof(StringValueObject).IsAssignableFrom(effectiveType))
        {
            return Activator.CreateInstance(effectiveType, rawValue);
        }

        MethodInfo? fromMethod = effectiveType.GetMethod(
            "From",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            new[] { typeof(string) },
            modifiers: null);

        if (fromMethod is not null)
        {
            return fromMethod.Invoke(null, new object[] { rawValue });
        }

        if (effectiveType.IsEnum)
        {
            return Enum.Parse(effectiveType, rawValue, ignoreCase: true);
        }

        if (effectiveType == typeof(Guid))
        {
            return Guid.Parse(rawValue);
        }

        if (effectiveType == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        return Convert.ChangeType(rawValue, effectiveType, CultureInfo.InvariantCulture);
    }

    private static IQueryable<T> ApplyOrdering<T>(
        IQueryable<T> source,
        string methodName,
        LambdaExpression keySelector,
        Type keyType)
    {
        MethodInfo method = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == methodName && m.GetParameters().Length == 2);

        MethodInfo generic = method.MakeGenericMethod(typeof(T), keyType);
        object? result = generic.Invoke(null, new object[] { source, keySelector });

        return (IQueryable<T>)result!;
    }
}
