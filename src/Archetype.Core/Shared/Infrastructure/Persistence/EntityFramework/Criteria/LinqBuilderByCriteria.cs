using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;

using Archetype.Core.Shared.Domain.FiltersByCriteria;
using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Shared.Infrastructure.Persistence.EntityFramework.Criteria
{
    public static class LinqBuilderByCriteria
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> collection, Domain.FiltersByCriteria.Criteria criteria)
        {
            if (criteria.Filters?.Values.Any() != true)
            {
                return collection;
            }

            List<string> queries = [];
            List<object?> parameters = [];

            foreach (Filter filter in criteria.Filters.Values)
            {
                if (TryGetQueryByFilter(typeof(T), filter, parameters, out string query))
                {
                    queries.Add(query);
                }
            }

            if (queries.Count == 0)
            {
                return collection;
            }

            return collection.Where(string.Join(" && ", queries), parameters.ToArray());
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection,
            Domain.FiltersByCriteria.Criteria criteria)
        {
            Order? order = criteria.Order;

            if (order?.OrderBy.Value == null || order.OrderType == OrderType.NONE)
            {
                return collection;
            }

            switch (order.OrderType)
            {
                case OrderType.ASC:
                    return collection.OrderBy(order.OrderBy.Value);
                case OrderType.DESC:
                    return collection.OrderBy($"{order.OrderBy.Value} DESC");
            }

            return collection;
        }

        public static IQueryable<T> Limit<T>(this IQueryable<T> collection,
            Domain.FiltersByCriteria.Criteria criteria)
        {
            if (criteria.Limit == null || criteria.Limit.Value == 0)
            {
                return collection;
            }

            return collection.Take(criteria.Limit.GetValueOrDefault());
        }

        public static IQueryable<T> Offset<T>(this IQueryable<T> collection,
            Domain.FiltersByCriteria.Criteria criteria)
        {
            if (criteria.Offset == null)
            {
                return collection;
            }

            return collection.Skip(criteria.Offset.GetValueOrDefault());
        }


        private static bool TryGetQueryByFilter(Type rootType, Filter filter, List<object?> parameters, out string query)
        {
            ResolvedField resolvedField = GetResolvedField(rootType, filter.Field.Value);
            string fieldName = resolvedField.Path;
            Type targetType = resolvedField.FieldType ?? typeof(string);

            Type valueType = filter.Operator is FilterOperator.CONTAINS or FilterOperator.NOTCONTAINS
                ? typeof(string)
                : targetType;

            object typedValue = ConvertFilterValue(filter.Value.Value, valueType);
            int parameterIndex = parameters.Count;

            switch (filter.Operator)
            {
                case FilterOperator.EQUAL:
                    parameters.Add(typedValue);
                    query = $"{fieldName} == @{parameterIndex}";
                    return true;
                case FilterOperator.NOTEQUAL:
                    parameters.Add(typedValue);
                    query = $"{fieldName} != @{parameterIndex}";
                    return true;
                case FilterOperator.GT:
                    parameters.Add(typedValue);
                    query = $"{fieldName} > @{parameterIndex}";
                    return true;
                case FilterOperator.GTE:
                    parameters.Add(typedValue);
                    query = $"{fieldName} >= @{parameterIndex}";
                    return true;
                case FilterOperator.LT:
                    parameters.Add(typedValue);
                    query = $"{fieldName} < @{parameterIndex}";
                    return true;
                case FilterOperator.LTE:
                    parameters.Add(typedValue);
                    query = $"{fieldName} <= @{parameterIndex}";
                    return true;
                case FilterOperator.CONTAINS:
                    parameters.Add(typedValue);
                    query = $"{fieldName}.Contains(@{parameterIndex})";
                    return true;
                case FilterOperator.NOTCONTAINS:
                    parameters.Add(typedValue);
                    query = $"!{fieldName}.Contains(@{parameterIndex})";
                    return true;
            }

            query = string.Empty;
            return false;
        }

        private static readonly ConcurrentDictionary<(Type RootType, string FieldPath), ResolvedField> _fieldPathCache = new();

        private const string ValuePropertyName = nameof(IValueObject<object>.Value);

        private static ResolvedField GetResolvedField(Type rootType, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return new ResolvedField(fieldName, null);
            }

            return _fieldPathCache.GetOrAdd((rootType, fieldName), static key => BuildFieldResolution(key.RootType, key.FieldPath));
        }

        private static ResolvedField BuildFieldResolution(Type rootType, string fieldName)
        {
            string[] segments =
                fieldName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (segments.Length == 0)
            {
                return new ResolvedField(fieldName, rootType);
            }

            List<string> resolvedSegments = new(segments.Length * 2);
            Type? currentType = rootType;

            for (int index = 0; index < segments.Length; index++)
            {
                string rawSegment = segments[index];

                if (string.Equals(rawSegment, ValuePropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    resolvedSegments.Add(ValuePropertyName);
                    continue;
                }

                if (currentType == null)
                {
                    resolvedSegments.Add(rawSegment);
                    continue;
                }

                MemberInfo? member = GetMemberByName(currentType, rawSegment);

                if (member == null)
                {
                    resolvedSegments.Add(rawSegment);
                    currentType = null;
                    continue;
                }

                resolvedSegments.Add(member.Name);

                Type memberType = GetMemberType(member);
                Type normalizedMemberType = Nullable.GetUnderlyingType(memberType) ?? memberType;

                if (TryGetValueObjectPrimitiveType(normalizedMemberType, out Type? primitiveType))
                {
                    bool nextSegmentIsExplicitValue = index + 1 < segments.Length &&
                                                      string.Equals(segments[index + 1], ValuePropertyName, StringComparison.OrdinalIgnoreCase);

                    if (!nextSegmentIsExplicitValue)
                    {
                        resolvedSegments.Add(ValuePropertyName);
                    }

                    currentType = primitiveType;
                }
                else
                {
                    currentType = normalizedMemberType;
                }
            }

            string resolvedPath = string.Join('.', resolvedSegments);
            return new ResolvedField(resolvedPath, currentType);
        }

        private static MemberInfo? GetMemberByName(Type declaringType, string memberName)
        {
            return declaringType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(m => (m is PropertyInfo || m is FieldInfo) &&
                                      m.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));
        }

        private static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo property => property.PropertyType,
                FieldInfo field => field.FieldType,
                _ => typeof(object)
            };
        }

        private static bool TryGetValueObjectPrimitiveType(Type type, out Type? primitiveType)
        {
            Type? valueObjectInterface = type is { IsInterface: true, IsGenericType: true } &&
                                         type.GetGenericTypeDefinition() == typeof(IValueObject<>)
                ? type
                : type
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValueObject<>));

            if (valueObjectInterface == null)
            {
                PropertyInfo? valueProperty =
                    type.GetProperty(ValuePropertyName, BindingFlags.Instance | BindingFlags.Public);
                if (valueProperty == null)
                {
                    primitiveType = null;
                    return false;
                }

                primitiveType = valueProperty.PropertyType;
                return true;
            }

            primitiveType = valueObjectInterface.GenericTypeArguments[0];
            return true;
        }

        private static object ConvertFilterValue(string rawValue, Type targetType)
        {
            Type nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (nonNullableType == typeof(string))
            {
                return rawValue;
            }

            if (nonNullableType.IsEnum)
            {
                return Enum.Parse(nonNullableType, rawValue, ignoreCase: true);
            }

            if (nonNullableType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(rawValue, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            if (nonNullableType == typeof(DateTime))
            {
                return DateTime.Parse(rawValue, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            if (nonNullableType == typeof(Guid))
            {
                return Guid.Parse(rawValue);
            }

            if (nonNullableType == typeof(bool))
            {
                return bool.Parse(rawValue);
            }

            if (nonNullableType == typeof(decimal))
            {
                return decimal.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(double))
            {
                return double.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(float))
            {
                return float.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(long))
            {
                return long.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(int))
            {
                return int.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(short))
            {
                return short.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(byte))
            {
                return byte.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(sbyte))
            {
                return sbyte.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(ushort))
            {
                return ushort.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(uint))
            {
                return uint.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            if (nonNullableType == typeof(ulong))
            {
                return ulong.Parse(rawValue, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(rawValue, nonNullableType, CultureInfo.InvariantCulture);
        }

        private sealed record ResolvedField(string Path, Type? FieldType);
    }
}
