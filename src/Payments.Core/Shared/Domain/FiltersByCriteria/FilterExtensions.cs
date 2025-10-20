namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public static class FilterExtensions
{
    public static List<Dictionary<string, string>> Only(this List<Dictionary<string, string>> filters, params string[] allowedFields)
    {
        HashSet<string> allowedFieldsSet = new(allowedFields.Select(f => f.ToLowerInvariant()), StringComparer.OrdinalIgnoreCase);

        return filters.Where(filter =>
            filter.TryGetValue("field", out string? fieldName) &&
            allowedFieldsSet.Contains(fieldName.ToLowerInvariant())).ToList();
    }
}
