using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Application;
using Payments.Core.Users.Domain;
using Payments.Tests.Shared.Domain;

namespace Payments.Tests.Users.Application;

public class GetUserHandlerTests
{
    [Fact]
    public async Task Find_WhenUserExists_ShouldReturnResponse()
    {
        var repository = new FakeUserRepository();
        var user = CreateUser();
        await repository.Save(user);
        var handler = new GetUserHandler(repository);

        var response = await handler.Find(user.Id.Value);

        Assert.NotNull(response);
        Assert.Equal(user.Id.Value, response.Id);
        Assert.Equal(user.Email.Value, response.Email);
        Assert.Equal(user.FullName.Value, response.FullName);
        Assert.Equal(new[] { user.Id }, repository.FindCalls);
    }

    [Fact]
    public async Task Find_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var repository = new FakeUserRepository();
        var handler = new GetUserHandler(repository);
        var missingId = Uuid.New();

        var response = await handler.Find(missingId.Value);

        Assert.Null(response);
        Assert.Equal(new[] { missingId }, repository.FindCalls);
    }

    private static User CreateUser()
    {
        var hasher = new FakeHasher();
        var id = Uuid.New();
        var email = new EmailAddress("john.doe@example.com");
        var fullName = new UserFullName("John Doe");
        var password = UserPasswordHash.Create("Valid12!", hasher);

        return User.Create(id, email, fullName, password);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly Dictionary<Uuid, User> _users = new();
        public List<Uuid> FindCalls { get; } = new();

        public Task<User?> Find(Uuid id)
        {
            FindCalls.Add(id);
            _users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task Save(User user)
        {
            _users[user.Id] = user;
            return Task.CompletedTask;
        }
    }
}
