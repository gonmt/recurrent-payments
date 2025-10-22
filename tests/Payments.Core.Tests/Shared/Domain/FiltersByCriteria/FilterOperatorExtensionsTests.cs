using Payments.Core.Shared.Domain.FiltersByCriteria;

namespace Payments.Core.Tests.Shared.Domain.FiltersByCriteria;

public class FilterOperatorExtensionsTests
{
    [Theory]
    [InlineData("=", FilterOperator.EQUAL)]
    [InlineData("!=", FilterOperator.NOTEQUAL)]
    [InlineData(">", FilterOperator.GT)]
    [InlineData(">=", FilterOperator.GTE)]
    [InlineData("<", FilterOperator.LT)]
    [InlineData("<=", FilterOperator.LTE)]
    [InlineData("CONTAINS", FilterOperator.CONTAINS)]
    [InlineData("NOT_CONTAINS", FilterOperator.NOTCONTAINS)]
    public void FilterOperatorFromValueReturnsExpectedOperator(string rawOperator, FilterOperator expected)
    {
        FilterOperator result = rawOperator.FilterOperatorFromValue();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void FilterOperatorFromValueWithUnsupportedOperatorThrows()
    {
        const string unsupported = "starts_with";

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => unsupported.FilterOperatorFromValue());

        Assert.Equal(unsupported, exception.ActualValue);
    }

    [Theory]
    [InlineData(FilterOperator.EQUAL, true)]
    [InlineData(FilterOperator.CONTAINS, true)]
    [InlineData(FilterOperator.GT, true)]
    [InlineData(FilterOperator.NOTCONTAINS, false)]
    [InlineData(FilterOperator.NOTEQUAL, false)]
    public void IsPositiveReturnsExpectedFlag(FilterOperator @operator, bool expected)
    {
        Assert.Equal(expected, @operator.IsPositive());
    }
}
