using FluentValidation;

namespace Archetype.Api.Endpoints.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
