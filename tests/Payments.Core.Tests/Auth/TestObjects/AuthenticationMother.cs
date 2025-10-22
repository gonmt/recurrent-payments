using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Tests.Users.TestObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Auth.TestObjects;

public static class AuthenticationMother
{
    public const string ValidPassword = "Valid12!";
    public const string IncorrectPassword = "Wrong12!";

    public static (User User, FakeHasher Hasher, string Password) SuccessfulAuthenticationScenario()
    {
        FakeHasher hasher = new();
        string password = ValidPassword;
        User user = UserMother.RandomWith(hasher: hasher);

        return (user, hasher, password);
    }

    public static (FakeHasher Hasher, string Email, string Password) UserNotExistsScenario()
    {
        FakeHasher hasher = new();
        string email = "nonexistent@example.com";
        string password = ValidPassword;

        return (hasher, email, password);
    }

    public static (User User, FakeHasher Hasher, string Password) HasherFailureScenario()
    {
        FakeHasher hasher = new(verifyFunc: (_, _) => false);
        string password = ValidPassword;
        User user = UserMother.RandomWith(hasher: hasher);

        return (user, hasher, password);
    }
}
