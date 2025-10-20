namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public class Filters(IEnumerable<Filter> filters)
{
    private readonly List<Filter> _filters = [.. filters];

    public IReadOnlyList<Filter> Values => _filters;

    public static Filters? FromValues(List<Dictionary<string, string>>? rawFilters)
    {
        return rawFilters is null ? null : new Filters(rawFilters.Select(Filter.FromValues));
    }
}
