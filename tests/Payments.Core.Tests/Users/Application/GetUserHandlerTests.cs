using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Application;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Application;

public class GetUserHandlerTests
{
    [Fact]
    public async Task FindWhenUserExistsShouldReturnResponse()
    {
        FakeUserRepository repository = new FakeUserRepository();
        User user = CreateUser();
        await repository.Save(user);
        GetUserHandler handler = new GetUserHandler(repository);

        GetUserResponse? response = await handler.Find(user.Id.Value);

        Assert.NotNull(response);
        Assert.Equal(user.Id.Value, response.Id);
        Assert.Equal(user.Email.Value, response.Email);
        Assert.Equal(user.FullName.Value, response.FullName);
        Assert.Equal(new[] { user.Id }, repository.FindCalls);
    }

    [Fact]
    public async Task FindWhenUserDoesNotExistShouldReturnNull()
    {
        FakeUserRepository repository = new FakeUserRepository();
        GetUserHandler handler = new GetUserHandler(repository);
        Uuid missingId = Uuid.New();

        GetUserResponse? response = await handler.Find(missingId.Value);

        Assert.Null(response);
        Assert.Equal(new[] { missingId }, repository.FindCalls);
    }

    private static User CreateUser()
    {
        FakeHasher hasher = new FakeHasher();
        Uuid id = Uuid.New();
        EmailAddress email = new EmailAddress("john.doe@example.com");
        UserFullName fullName = new UserFullName("John Doe");
        UserPasswordHash password = UserPasswordHash.Create("Valid12!", hasher);

        return User.Create(id, email, fullName, password);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly Dictionary<Uuid, User> _users = new();
        public List<Uuid> FindCalls { get; } = new();

        public Task<User?> Find(Uuid id)
        {
            FindCalls.Add(id);
            _users.TryGetValue(id, out User? user);
            return Task.FromResult(user);
        }

        public Task<User?> FindByEmail(EmailAddress email)
        {
            User? user = _users.Values.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user);
        }

        public Task Save(User user)
        {
            _users[user.Id] = user;
            return Task.CompletedTask;
        }
    }
}
