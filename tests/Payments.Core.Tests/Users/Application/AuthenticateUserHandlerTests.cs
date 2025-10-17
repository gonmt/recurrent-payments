using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Application;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Application;

public class AuthenticateUserHandlerTests
{
    [Fact]
    public async Task AuthenticateWithValidCredentialsShouldReturnUserSummary()
    {
        FakeHasher hasher = new();
        InMemoryRepository repository = new(hasher);
        AuthenticateUserHandler handler = new(repository, hasher);

        AuthenticateUserResponse? response = await handler.Authenticate("user@example.com", "Valid12!");

        Assert.NotNull(response);
        Assert.Equal(repository.StoredUser.Id.Value, response.Id);
        Assert.Equal(repository.StoredUser.Email.Value, response.Email);
        Assert.Equal(repository.StoredUser.FullName.Value, response.FullName);
    }

    [Fact]
    public async Task AuthenticateWhenUserDoesNotExistShouldReturnNull()
    {
        FakeHasher hasher = new();
        InMemoryRepository repository = new(hasher, includeUser: false);
        AuthenticateUserHandler handler = new(repository, hasher);

        AuthenticateUserResponse? response = await handler.Authenticate("missing@example.com", "SomePass1!");

        Assert.Null(response);
    }

    [Fact]
    public async Task AuthenticateWithIncorrectPasswordShouldReturnNull()
    {
        FakeHasher hasher = new((plain) => $"{plain}_HASH", (plain, hash) => false);
        InMemoryRepository repository = new(hasher);
        AuthenticateUserHandler handler = new(repository, hasher);

        AuthenticateUserResponse? response = await handler.Authenticate("user@example.com", "WrongPass1!");

        Assert.Null(response);
    }

    private sealed class InMemoryRepository : IUserRepository
    {
        private readonly Dictionary<EmailAddress, User> _users = new();
        public User StoredUser { get; }

        public InMemoryRepository(FakeHasher hasher, bool includeUser = true)
        {
            if (includeUser)
            {
                UserPasswordHash password = UserPasswordHash.Create("Valid12!", hasher);
                Uuid id = Uuid.New();
                EmailAddress email = new EmailAddress("user@example.com");
                UserFullName fullName = new UserFullName("Jane Doe");

                StoredUser = User.Create(id, email, fullName, password);
                _users[email] = StoredUser;
            }
            else
            {
                StoredUser = null!;
            }
        }

        public Task<User?> Find(Uuid id)
        {
            User? user = _users.Values.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User?> FindByEmail(EmailAddress email)
        {
            _users.TryGetValue(email, out User? user);
            return Task.FromResult(user);
        }

        public Task Save(User user)
        {
            _users[user.Email] = user;
            return Task.CompletedTask;
        }
    }
}
