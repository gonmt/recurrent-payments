namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public class Criteria(Filters? filters, Order? order, int? limit = null, int? offset = null)
{
    public Filters? Filters { get; } = filters;
    public Order? Order { get; } = order;
    public int? Limit { get; } = limit;
    public int? Offset { get; } = offset;

    public bool HasFilters() => Filters is { Values.Count: > 0 };

    public bool HasOrder() => Order is { OrderType: not OrderType.NONE } &&
        !string.IsNullOrEmpty(Order.OrderBy?.Value);
}
