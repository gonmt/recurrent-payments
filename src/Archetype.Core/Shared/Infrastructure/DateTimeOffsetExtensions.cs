using System.Globalization;

namespace Archetype.Core.Shared.Infrastructure;

public static class DateTimeOffsetExtensions
{
    private const string DefaultDateTimeFormat = "O";

    public static string ToApplicationString(this DateTimeOffset value) =>
        value.ToString(DefaultDateTimeFormat, CultureInfo.InvariantCulture);
}
