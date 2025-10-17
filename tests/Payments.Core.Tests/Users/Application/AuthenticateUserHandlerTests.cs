using Bogus;

using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Application;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Application;

public class AuthenticateUserHandlerTests
{
    private static readonly Faker _faker = new();

    [Fact]
    public async Task AuthenticateWithValidCredentialsShouldReturnUserSummary()
    {
        FakeHasher hasher = new();
        string password = GenerateValidPassword();
        User user = BuildUser(hasher, password);
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
        FakeHasher hasher = new();
        InMemoryRepository repository = new(null);
        AuthenticateUserHandler handler = new(repository, hasher);

        AuthenticateUserResponse? response = await handler.Authenticate(_faker.Internet.Email(), GenerateValidPassword());

        Assert.Null(response);
    }

    [Fact]
    public async Task AuthenticateWithIncorrectPasswordShouldReturnNull()
    {
        FakeHasher hasher = new(hashFunc: plain => $"{plain}_HASH", verifyFunc: (_, _) => false);
        string password = GenerateValidPassword();
        User user = BuildUser(hasher, password);
        InMemoryRepository repository = new(user);
        AuthenticateUserHandler handler = new(repository, hasher);

        AuthenticateUserResponse? response = await handler.Authenticate(user.Email.Value, password);

        Assert.Null(response);
    }

    private static User BuildUser(IHasher hasher, string password)
    {
        return User.Create(
            Uuid.New(),
            new EmailAddress(_faker.Internet.Email()),
            new UserFullName(_faker.Person.FullName),
            UserPasswordHash.Create(password, hasher));
    }

    private static string GenerateValidPassword()
    {
        List<char> characters =
        [
            _faker.Random.Char('A', 'Z'),
            _faker.Random.Char('a', 'z'),
            _faker.Random.Char('0', '9'),
            _faker.Random.ArrayElement("!@#$%^&*".ToCharArray())
        ];

        int extraCharacters = _faker.Random.Int(4, 8);
        for (int i = 0; i < extraCharacters; i++)
        {
            characters.Add(_faker.Random.ArrayElement("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*".ToCharArray()));
        }

        _ = _faker.Random.Shuffle(characters);
        return new string(characters.ToArray());
    }

    private sealed class InMemoryRepository : IUserRepository
    {
        private readonly Dictionary<EmailAddress, User> _byEmail = [];
        private readonly Dictionary<Uuid, User> _byId = new();

        public InMemoryRepository(User? user)
        {
            if (user is not null)
            {
                _byEmail[user.Email] = user;
                _byId[user.Id] = user;
            }
        }

        public Task<User?> Find(Uuid id)
        {
            _ = _byId.TryGetValue(id, out User? user);
            return Task.FromResult(user);
        }

        public Task<User?> FindByEmail(EmailAddress email)
        {
            _ = _byEmail.TryGetValue(email, out User? user);
            return Task.FromResult(user);
        }

        public Task Save(User user)
        {
            _byEmail[user.Email] = user;
            _byId[user.Id] = user;
            return Task.CompletedTask;
        }
    }
}
