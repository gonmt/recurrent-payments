using Microsoft.Extensions.Primitives;

namespace Payments.Api.Endpoints.Shared;

public class QueryProcessor(IHttpContextAccessor httpContextAccessor)
{
    private readonly IHttpContextAccessor? _httpContextAccessor = httpContextAccessor;
    private List<Dictionary<string, string>>? _filters;
    private readonly Lock _lock = new();

    private IQueryCollection QueryParams =>
        _httpContextAccessor?.HttpContext?.Request.Query ??
        new QueryCollection();

    /// <summary>
    /// Gets all parsed filters from query parameters (except pagination/sorting params)
    /// </summary>
    public List<Dictionary<string, string>> Filters
    {
        get
        {
            if (_filters != null)
            {
                return _filters ?? [];
            }

            lock (_lock)
            {
                _filters ??= ParseFilters();
            }

            return _filters ?? [];
        }
    }

    /// <summary>
    /// Gets the order by field name
    /// </summary>
    public string? OrderBy => GetOrderBy();

    /// <summary>
    /// Gets the sort order (asc/desc)
    /// </summary>
    public string? Order => GetOrder();

    /// <summary>
    /// Gets the pagination limit
    /// </summary>
    public int? Limit => GetLimit();

    /// <summary>
    /// Gets the pagination offset
    /// </summary>
    public int? Offset => GetOffset();

    private List<Dictionary<string, string>> ParseFilters()
    {
        List<Dictionary<string, string>> filters = [];
        HashSet<string> excludedParams = new(StringComparer.OrdinalIgnoreCase)
        {
            "orderby", "order", "limit", "offset"
        };

        Dictionary<string, string> operatorMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { "eq", "=" },
            { "equals", "=" },
            { "ne", "!=" },
            { "not-equals", "!=" },
            { "contains", "CONTAINS" },
            { "not-contains", "NOT_CONTAINS" },
            { "gt", ">" },
            { "gte", ">=" },
            { "lt", "<" },
            { "lte", "<=" }
        };

        foreach (KeyValuePair<string, StringValues> param in QueryParams)
        {
            if (excludedParams.Contains(param.Key))
            {
                continue;
            }

            foreach (string? value in param.Value)
            {
                string filterString = $"{param.Key}={value}";
                Dictionary<string, string>? parsedFilter = ParseSingleFilter(filterString, operatorMappings);
                if (parsedFilter != null)
                {
                    filters.Add(parsedFilter);
                }
            }
        }

        return filters;
    }

    private static Dictionary<string, string>? ParseSingleFilter(string filter,
        Dictionary<string, string> operatorMappings)
    {
        int equalIndex = filter.IndexOf('=');
        if (equalIndex == -1)
        {
            return null;
        }

        string leftSide = filter.Substring(0, equalIndex);
        string value = filter.Substring(equalIndex + 1);

        if (string.IsNullOrWhiteSpace(leftSide) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        int dashIndex = leftSide.LastIndexOf('-');
        string field, operatorKey;

        if (dashIndex == -1)
        {
            field = leftSide;
            operatorKey = "contains";
        }
        else
        {
            field = leftSide.Substring(0, dashIndex);
            operatorKey = leftSide.Substring(dashIndex + 1);
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            return null;
        }

        string operatorValue = operatorMappings.GetValueOrDefault(operatorKey, "CONTAINS");

        return new Dictionary<string, string>
        {
            ["field"] = field,
            ["operator"] = operatorValue,
            ["value"] = value
        };
    }

    private string? GetOrderBy() => QueryParams.TryGetValue("orderBy", out StringValues orderByValues) ? orderByValues.FirstOrDefault() : null;

    private string? GetOrder() => QueryParams.TryGetValue("order", out StringValues orderValues) ? orderValues.FirstOrDefault() : null;

    private int? GetLimit()
    {
        return QueryParams.TryGetValue("limit", out StringValues limitValues) &&
            int.TryParse(limitValues.FirstOrDefault(), out int limit)
            ? limit
            : null;
    }

    private int? GetOffset()
    {
        return QueryParams.TryGetValue("offset", out StringValues offsetValues) &&
            int.TryParse(offsetValues.FirstOrDefault(), out int offset)
            ? offset
            : null;
    }
}
