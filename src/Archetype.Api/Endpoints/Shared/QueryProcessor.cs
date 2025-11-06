using System.Text;

using Microsoft.Extensions.Primitives;

namespace Archetype.Api.Endpoints.Shared;

public class QueryProcessor(IHttpContextAccessor httpContextAccessor)
{
    private readonly IHttpContextAccessor? _httpContextAccessor = httpContextAccessor;
    private List<Dictionary<string, string>>? _filters;
    private readonly Lock _lock = new();

    private IQueryCollection QueryParams =>
        _httpContextAccessor?.HttpContext?.Request.Query ??
        new QueryCollection();

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

    public string? OrderBy => GetOrderBy();

    public string? Order => GetOrder();

    public int? Limit => GetLimit();

    public int? Offset => GetOffset();

    private List<Dictionary<string, string>> ParseFilters()
    {
        List<Dictionary<string, string>> filters = [];
        HashSet<string> excludedParams = new(StringComparer.OrdinalIgnoreCase)
        {
            "order", "limit", "offset", "orderby", "order_by"
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

        string field;
        string operatorKey;

        string[] segments = leftSide.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (segments.Length == 0)
        {
            return null;
        }

        field = segments[0];
        operatorKey = segments.Length > 1 ? string.Join('-', segments.Skip(1)) : "eq";

        if (string.IsNullOrWhiteSpace(field))
        {
            return null;
        }

        field = ToPascalCase(field);

        string operatorValue = operatorMappings.GetValueOrDefault(operatorKey, "CONTAINS");

        return new Dictionary<string, string>
        {
            ["field"] = field,
            ["operator"] = operatorValue,
            ["value"] = value
        };
    }

    private string? GetOrderBy()
    {
        if (TryGetQueryValue("order_by", out string? snakeOrderBy) && !string.IsNullOrWhiteSpace(snakeOrderBy))
        {
            return ToPascalCase(snakeOrderBy);
        }

        if (TryGetQueryValue("orderBy", out string? legacyOrderBy) && !string.IsNullOrWhiteSpace(legacyOrderBy))
        {
            return ToPascalCase(legacyOrderBy);
        }

        return null;
    }

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

    private bool TryGetQueryValue(string key, out string? value)
    {
        if (QueryParams.TryGetValue(key, out StringValues values))
        {
            value = values.FirstOrDefault();
            return true;
        }

        value = null;
        return false;
    }

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        StringBuilder builder = new(value.Length);
        bool capitalizeNext = true;

        foreach (char c in value)
        {
            if (c == '_' || c == '-' || c == ' ')
            {
                capitalizeNext = true;
                continue;
            }

            if (capitalizeNext)
            {
                builder.Append(char.ToUpperInvariant(c));
                capitalizeNext = false;
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}
