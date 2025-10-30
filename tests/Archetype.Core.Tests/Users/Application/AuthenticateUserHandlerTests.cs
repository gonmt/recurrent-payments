using Archetype.Core.Shared.Domain.Results;
using Archetype.Core.Tests.Auth.TestObjects;
using Archetype.Core.Tests.Users.TestObjects;
using Archetype.Core.Users.Application;
using Archetype.Core.Users.Domain;

namespace Archetype.Core.Tests.Users.Application;

public class AuthenticateUserHandlerTests : UsersTestBase
{

    [Fact]
    public async Task AuthenticateWithValidCredentialsShouldReturnUserSummary()
    {

        (User user, Shared.Domain.FakeHasher hasher, string password) = AuthenticationMother.SuccessfulAuthenticationScenario();
        InMemoryRepository repository = new(user);
        AuthenticateUserHandler handler = new(repository, hasher);


        Result<AuthenticateUserResponse> result = await handler.Authenticate(user.Email.Value, password);


        Assert.True(result.IsSuccess);
        AuthenticateUserResponse response = result.Value!;
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


        Result<AuthenticateUserResponse> result = await handler.Authenticate(email, password);


        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Unauthorized, result.FirstError.Type);
    }

    [Fact]
    public async Task AuthenticateWithIncorrectPasswordShouldReturnNull()
    {

        (User user, Shared.Domain.FakeHasher hasher, string password) = AuthenticationMother.HasherFailureScenario();
        InMemoryRepository repository = new(user);
        AuthenticateUserHandler handler = new(repository, hasher);


        Result<AuthenticateUserResponse> result = await handler.Authenticate(user.Email.Value, password);


        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Unauthorized, result.FirstError.Type);
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
