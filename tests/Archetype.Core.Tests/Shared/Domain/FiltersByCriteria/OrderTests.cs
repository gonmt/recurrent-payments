using Archetype.Core.Shared.Domain.FiltersByCriteria;

namespace Archetype.Core.Tests.Shared.Domain.FiltersByCriteria;

public class OrderTests
{
    [Fact]
    public void FromValuesWithEmptyOrderTypeReturnsNone()
    {
        Order order = Order.FromValues("createdAt", "");

        Assert.Equal(OrderType.NONE, order.OrderType);
        Assert.Equal("createdAt", order.OrderBy.Value);
    }

    [Theory]
    [InlineData("asc", OrderType.ASC)]
    [InlineData("DESC", OrderType.DESC)]
    public void FromValuesParsesOrderTypeIgnoringCase(string rawOrder, OrderType expected)
    {
        Order order = Order.FromValues("createdAt", rawOrder);

        Assert.Equal(expected, order.OrderType);
        Assert.Equal("createdAt", order.OrderBy.Value);
    }
}
