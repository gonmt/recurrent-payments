using Payments.Core.Shared.Domain.FiltersByCriteria;

namespace Payments.Core.Tests.Shared.Domain.FiltersByCriteria;

public class CriteriaTests
{
    [Fact]
    public void HasFiltersWhenFiltersAreNullReturnsFalse()
    {
        Criteria criteria = new(filters: null, order: Order.None());

        Assert.False(criteria.HasFilters());
    }

    [Fact]
    public void HasFiltersWhenFiltersCollectionEmptyReturnsFalse()
    {
        Filters emptyFilters = new(new List<Filter>());
        Criteria criteria = new(emptyFilters, Order.None());

        Assert.False(criteria.HasFilters());
    }

    [Fact]
    public void HasFiltersWhenFiltersPresentReturnsTrue()
    {
        Filters filters = new(new List<Filter>
        {
            new(new FilterField("email"), FilterOperator.EQUAL, new FilterValue("alice@example.com"))
        });

        Criteria criteria = new(filters, Order.None());

        Assert.True(criteria.HasFilters());
    }

    [Theory]
    [InlineData("createdAt", OrderType.DESC, true)]
    [InlineData("", OrderType.DESC, false)]
    [InlineData("createdAt", OrderType.NONE, false)]
    public void HasOrderReturnsExpectedResult(string orderBy, OrderType orderType, bool expected)
    {
        Order order = new(new OrderBy(orderBy), orderType);
        Criteria criteria = new(filters: null, order, limit: null, offset: null);

        Assert.Equal(expected, criteria.HasOrder());
    }
}
