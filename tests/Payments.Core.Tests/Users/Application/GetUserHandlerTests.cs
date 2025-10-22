using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Users.TestObjects;
using Payments.Core.Users.Application;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Application;

public class GetUserHandlerTests : UsersTestBase
{
    [Fact]
    public async Task FindWhenUserExistsShouldReturnResponse()
    {

        FakeUserRepository repository = new();
        User user = UserMother.Random();
        await repository.Save(user);
        GetUserHandler handler = new(repository);


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
        FakeUserRepository repository = new();
        GetUserHandler handler = new(repository);
        Uuid missingId = Uuid.New();

        GetUserResponse? response = await handler.Find(missingId.Value);

        Assert.Null(response);
        Assert.Equal(new[] { missingId }, repository.FindCalls);
    }

    private sealed class FakeUserRepository : TestUserRepositoryBase
    {
        public List<Uuid> FindCalls { get; } = new();

        public override Task<User?> Find(Uuid id)
        {
            FindCalls.Add(id);
            return base.Find(id);
        }
    }
}
