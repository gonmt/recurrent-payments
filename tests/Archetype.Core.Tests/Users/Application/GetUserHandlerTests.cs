using Archetype.Core.Shared.Domain.Results;
using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Tests.Users.TestObjects;
using Archetype.Core.Users.Application;
using Archetype.Core.Users.Domain;

namespace Archetype.Core.Tests.Users.Application;

public class GetUserHandlerTests : UsersTestBase
{
    [Fact]
    public async Task FindWhenUserExistsShouldReturnResponse()
    {

        FakeUserRepository repository = new();
        User user = UserMother.Random();
        await repository.Save(user);
        GetUserHandler handler = new(repository);


        Result<GetUserResponse> result = await handler.Find(user.Id.Value);


        Assert.True(result.IsSuccess);
        GetUserResponse response = result.Value!;
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

        Result<GetUserResponse> result = await handler.Find(missingId.Value);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
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
