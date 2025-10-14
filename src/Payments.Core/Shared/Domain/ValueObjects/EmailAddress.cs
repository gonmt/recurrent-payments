using System.Text.RegularExpressions;

namespace Payments.Core.Shared.Domain.ValueObjects;

public sealed record EmailAddress : StringValueObject
{
    public EmailAddress(string value) : base(value) { }
    
    protected override void Validate(string emailAddress)
    {
        if (!Regex.IsMatch(emailAddress, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email address.", nameof(emailAddress));
    }
}
