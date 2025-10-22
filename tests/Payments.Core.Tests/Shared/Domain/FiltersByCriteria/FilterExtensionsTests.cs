using Payments.Core.Shared.Domain.FiltersByCriteria;

namespace Payments.Core.Tests.Shared.Domain.FiltersByCriteria;

public class FilterExtensionsTests
{
    [Fact]
    public void OnlyReturnsFiltersMatchingAllowedFieldsIgnoringCase()
    {
        List<Dictionary<string, string>> filters = new()
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
                ["value"] = "alice"
            },
            new()
            {
                ["field"] = "CreatedAt",
                ["operator"] = ">",
                ["value"] = "2024-01-01"
            }
        };

        List<Dictionary<string, string>> result = filters.Only("email", "fullname");

        Assert.Collection(result,
            filter => Assert.Equal("Email", filter["field"]),
            filter => Assert.Equal("FullName", filter["field"]));
    }

    [Fact]
    public void OnlyWhenAllowedFieldsEmptyReturnsEmptyList()
    {
        List<Dictionary<string, string>> filters = new()
        {
            new()
            {
                ["field"] = "Email",
                ["operator"] = "=",
                ["value"] = "alice@example.com"
            }
        };

        List<Dictionary<string, string>> result = filters.Only();

        Assert.Empty(result);
    }
}
