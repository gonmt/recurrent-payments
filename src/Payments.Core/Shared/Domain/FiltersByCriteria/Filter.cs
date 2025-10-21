namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public class Filter(FilterField field, FilterOperator @operator, FilterValue value)
{
    public FilterField Field { get; } = field;
    public FilterOperator Operator { get; } = @operator;
    public FilterValue Value { get; } = value;

    public static Filter FromValues(Dictionary<string, string> values)
    {

        return new Filter(
            new FilterField(values["field"]),
            values["operator"].FilterOperatorFromValue(),
            new FilterValue(values["value"]));
    }
}
