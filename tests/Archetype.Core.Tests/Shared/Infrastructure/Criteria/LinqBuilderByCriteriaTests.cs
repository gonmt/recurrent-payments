using System.Globalization;
using System.Reflection;

using Archetype.Core.Shared.Infrastructure.Persistence.EntityFramework.Criteria;
using Archetype.Core.Users.Domain;

using DomainCriteria = Archetype.Core.Shared.Domain.FiltersByCriteria.Criteria;
using DomainFilters = Archetype.Core.Shared.Domain.FiltersByCriteria.Filters;

namespace Archetype.Core.Tests.Shared.Infrastructure.Criteria;

public class LinqBuilderByCriteriaTests
{
    [Fact]
    public void GetResolvedFieldAppendsValueForValueObjectFields()
    {
        MethodInfo? method = typeof(LinqBuilderByCriteria).GetMethod(
            "GetResolvedField",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        object? result = method!.Invoke(null, new object[] { typeof(User), "Email" });

        Assert.NotNull(result);

        PropertyInfo? pathProperty = result.GetType().GetProperty("Path");
        PropertyInfo? typeProperty = result.GetType().GetProperty("FieldType");

        Assert.NotNull(pathProperty);
        Assert.NotNull(typeProperty);

        Assert.Equal("Email.Value", pathProperty!.GetValue(result));
        Assert.Equal(typeof(string), typeProperty!.GetValue(result));
    }

    [Fact]
    public void WhereWithEqualOperatorFiltersMatches()
    {
        IQueryable<SampleEntity> data = new List<SampleEntity>
        {
            new("alpha", 10),
            new("beta", 20)
        }.AsQueryable();

        DomainCriteria criteria = BuildCriteria("Text", "=", "alpha");

        List<SampleEntity> result = data.Where(criteria).ToList();

        SampleEntity entity = Assert.Single(result);
        Assert.Equal("alpha", entity.Text);
    }

    [Fact]
    public void WhereWithDateTimeOffsetComparisonFiltersCorrectly()
    {
        DateTimeOffset baseDate = new(2025, 10, 1, 0, 0, 0, TimeSpan.Zero);

        IQueryable<DateEntity> data = new List<DateEntity>
        {
            new(baseDate.AddDays(-1)),
            new(baseDate),
            new(baseDate.AddDays(1))
        }.AsQueryable();

        DomainCriteria criteria = BuildCriteria("Timestamp", ">=", baseDate.ToString("o", CultureInfo.InvariantCulture));

        List<DateEntity> result = data.Where(criteria).ToList();

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, r => r.Timestamp < baseDate);
    }

    [Fact]
    public void WhereWithNotEqualOperatorFiltersOutMatches()
    {
        IQueryable<SampleEntity> data = new List<SampleEntity>
        {
            new("alpha", 10),
            new("beta", 20)
        }.AsQueryable();

        DomainCriteria criteria = BuildCriteria("Text", "!=", "alpha");

        List<SampleEntity> result = data.Where(criteria).ToList();

        SampleEntity entity = Assert.Single(result);
        Assert.Equal("beta", entity.Text);
    }

    [Fact]
    public void WhereWithContainsOperatorFindsPartialMatches()
    {
        IQueryable<SampleEntity> data = new List<SampleEntity>
        {
            new("alpha", 10),
            new("beta", 20)
        }.AsQueryable();

        DomainCriteria criteria = BuildCriteria("Text", "CONTAINS", "ph");

        List<SampleEntity> result = data.Where(criteria).ToList();

        SampleEntity entity = Assert.Single(result);
        Assert.Equal("alpha", entity.Text);
    }

    [Fact]
    public void WhereWithNotContainsOperatorExcludesPartialMatches()
    {
        IQueryable<SampleEntity> data = new List<SampleEntity>
        {
            new("alpha", 10),
            new("beta", 20)
        }.AsQueryable();

        DomainCriteria criteria = BuildCriteria("Text", "NOT_CONTAINS", "ph");

        List<SampleEntity> result = data.Where(criteria).ToList();

        SampleEntity entity = Assert.Single(result);
        Assert.Equal("beta", entity.Text);
    }

    [Fact]
    public void WhereWithGreaterOperatorsFiltersNumerics()
    {
        IQueryable<SampleEntity> data = new List<SampleEntity>
        {
            new("alpha", 10),
            new("beta", 20)
        }.AsQueryable();

        DomainCriteria gtCriteria = BuildCriteria("Number", ">", "15");
        DomainCriteria gteCriteria = BuildCriteria("Number", ">=", "20");

        List<SampleEntity> gtResult = data.Where(gtCriteria).ToList();
        SampleEntity gtEntity = Assert.Single(gtResult);
        Assert.Equal(20, gtEntity.Number);

        List<SampleEntity> gteResult = data.Where(gteCriteria).ToList();
        SampleEntity gteEntity = Assert.Single(gteResult);
        Assert.Equal(20, gteEntity.Number);
    }

    [Fact]
    public void WhereWithLessOperatorsFiltersNumerics()
    {
        IQueryable<SampleEntity> data = new List<SampleEntity>
        {
            new("alpha", 10),
            new("beta", 20)
        }.AsQueryable();

        DomainCriteria ltCriteria = BuildCriteria("Number", "<", "15");
        DomainCriteria lteCriteria = BuildCriteria("Number", "<=", "20");

        List<SampleEntity> ltResult = data.Where(ltCriteria).ToList();
        SampleEntity ltEntity = Assert.Single(ltResult);
        Assert.Equal(10, ltEntity.Number);

        List<SampleEntity> lteResult = data.Where(lteCriteria).ToList();
        Assert.Equal(2, lteResult.Count);
    }

    private static DomainCriteria BuildCriteria(string field, string @operator, string value)
    {
        Dictionary<string, string> rawFilter = new(StringComparer.OrdinalIgnoreCase)
        {
            ["field"] = field,
            ["operator"] = @operator,
            ["value"] = value
        };

        DomainFilters? filters = DomainFilters.FromValues([rawFilter]);
        return new DomainCriteria(filters, null);
    }

    private sealed record SampleEntity(string Text, int Number);
    private sealed record DateEntity(DateTimeOffset Timestamp);
}
