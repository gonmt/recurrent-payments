namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public enum FilterOperator
{
    EQUAL,
    NOTEQUAL,
    GT,
    GTE,
    LT,
    LTE,
    CONTAINS,
    NOTCONTAINS
}

public static class FilterOperatorExtensions
{
    public static FilterOperator FilterOperatorFromValue(this string value)
    {
        return value switch
        {
            "=" => FilterOperator.EQUAL,
            "!=" => FilterOperator.NOTEQUAL,
            ">" => FilterOperator.GT,
            ">=" => FilterOperator.GTE,
            "<" => FilterOperator.LT,
            "<=" => FilterOperator.LTE,
            "CONTAINS" => FilterOperator.CONTAINS,
            "NOT_CONTAINS" => FilterOperator.NOTCONTAINS,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported filter operator"),
        };
    }

    public static bool IsPositive(this FilterOperator value) =>
        value is not (FilterOperator.NOTCONTAINS or FilterOperator.NOTEQUAL);
}
