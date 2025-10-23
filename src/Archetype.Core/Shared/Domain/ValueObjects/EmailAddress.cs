using System.Text.RegularExpressions;

namespace Archetype.Core.Shared.Domain.ValueObjects;

public sealed partial record EmailAddress : StringValueObject
{
    public EmailAddress(string value)
        : base(value, validator: EnsureValidEmail)
    {
    }

    private static void EnsureValidEmail(string v)
    {
        if (!MyRegex().IsMatch(v))
        {
            throw new ArgumentException("Invalid email address.", nameof(v));
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex MyRegex();
}
