using System.Text.RegularExpressions;

namespace Payments.Core.Shared.Domain.ValueObjects
{
    public sealed partial record EmailAddress : StringValueObject
    {
        public EmailAddress(string value) : base(value) { }

        protected override void Validate(string emailAddress)
        {
            if (!MyRegex().IsMatch(emailAddress))
            {
                throw new ArgumentException("Invalid email address.", nameof(emailAddress));
            }
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex MyRegex();
    }
}
