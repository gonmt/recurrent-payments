using Payments.Core.Tests.Auth.TestObjects;
using Payments.Core.Tests.Users.TestObjects;
using Payments.Core.Users.Application;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Application;

public class AuthenticateUserHandlerTests : UsersTestBase
{

    [Fact]
    public async Task AuthenticateWithValidCredentialsShouldReturnUserSummary()
    {

        (User user, Shared.Domain.FakeHasher hasher, string password) = AuthenticationMother.SuccessfulAuthenticationScenario();
        InMemoryRepository repository = new(user);
        AuthenticateUserHandler handler = new(repository, hasher);


        AuthenticateUserResponse? response = await handler.Authenticate(user.Email.Value, password);


        Assert.NotNull(response);
        Assert.Equal(user.Id.Value, response.Id);
        Assert.Equal(user.Email.Value, response.Email);
        Assert.Equal(user.FullName.Value, response.FullName);
    }

    [Fact]
    public async Task AuthenticateWhenUserDoesNotExistShouldReturnNull()
    {

        (Shared.Domain.FakeHasher hasher, string email, string password) = AuthenticationMother.UserNotExistsScenario();
        InMemoryRepository repository = new(null);
        AuthenticateUserHandler handler = new(repository, hasher);


        AuthenticateUserResponse? response = await handler.Authenticate(email, password);


        Assert.Null(response);
    }

    [Fact]
    public async Task AuthenticateWithIncorrectPasswordShouldReturnNull()
    {

        (User user, Shared.Domain.FakeHasher hasher, string password) = AuthenticationMother.HasherFailureScenario();
        InMemoryRepository repository = new(user);
        AuthenticateUserHandler handler = new(repository, hasher);


        AuthenticateUserResponse? response = await handler.Authenticate(user.Email.Value, password);


        Assert.Null(response);
    }

    private sealed class InMemoryRepository : TestUserRepositoryBase
    {
        public InMemoryRepository(User? user = null)
        {
            if (user != null)
            {
                base.Save(user).Wait();
            }
        }
    }
}
