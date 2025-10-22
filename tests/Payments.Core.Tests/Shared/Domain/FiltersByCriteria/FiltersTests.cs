using Payments.Core.Shared.Domain.FiltersByCriteria;

namespace Payments.Core.Tests.Shared.Domain.FiltersByCriteria;

public class FiltersTests
{
    [Fact]
    public void FromValuesWithNullListReturnsNull()
    {
        Filters? filters = Filters.FromValues(null);

        Assert.Null(filters);
    }

    [Fact]
    public void FromValuesWithValidValuesBuildsFiltersCollection()
    {
        List<Dictionary<string, string>> rawFilters = new()
        {
            new()
            {
                ["field"] = "Email",
                ["operator"] = "=",
                ["value"] = "alice@example.com"
            },
            new()
            {
                ["field"] = "FullName",
                ["operator"] = "CONTAINS",
                ["value"] = "Alice"
            }
        };

        Filters? filters = Filters.FromValues(rawFilters);

        Assert.NotNull(filters);
        Assert.Collection(filters!.Values,
            filter =>
            {
                Assert.Equal("Email", filter.Field.Value);
                Assert.Equal(FilterOperator.EQUAL, filter.Operator);
                Assert.Equal("alice@example.com", filter.Value.Value);
            },
            filter =>
            {
                Assert.Equal("FullName", filter.Field.Value);
                Assert.Equal(FilterOperator.CONTAINS, filter.Operator);
                Assert.Equal("Alice", filter.Value.Value);
            });
    }
}
