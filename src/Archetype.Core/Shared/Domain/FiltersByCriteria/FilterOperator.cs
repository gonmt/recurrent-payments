namespace Archetype.Core.Shared.Domain.FiltersByCriteria;

public enum FilterOperator
{
    EQUAL = 0,
    NOTEQUAL = 1,
    GT = 2,
    GTE = 3,
    LT = 4,
    LTE = 5,
    CONTAINS = 6,
    NOTCONTAINS = 7
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
