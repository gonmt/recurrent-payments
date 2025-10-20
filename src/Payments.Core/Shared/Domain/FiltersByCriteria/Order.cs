namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public class Order(OrderBy orderBy, OrderType orderType)
{
    public OrderBy OrderBy { get; } = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
    public OrderType OrderType { get; } = orderType;

    public static Order FromValues(string orderBy, string orderType)
    {
        ArgumentNullException.ThrowIfNull(orderBy);

        OrderType parsedOrder = string.IsNullOrEmpty(orderType)
            ? OrderType.NONE
            : Enum.Parse<OrderType>(orderType.ToUpperInvariant(), true);

        return new Order(new OrderBy(orderBy), parsedOrder);
    }

    public static Order None() => new(new OrderBy(string.Empty), OrderType.NONE);
}
