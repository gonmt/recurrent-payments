namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public class Filter(FilterField field, FilterOperator @operator, FilterValue value)
{
    public FilterField Field { get; } = field ?? throw new ArgumentNullException(nameof(field));
    public FilterOperator Operator { get; } = @operator;
    public FilterValue Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

    public static Filter FromValues(Dictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return new Filter(
            new FilterField(values["field"]),
            values["operator"].FilterOperatorFromValue(),
            new FilterValue(values["value"]));
    }
}
