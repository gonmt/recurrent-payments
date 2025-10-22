using Payments.Core.Shared.Domain.FiltersByCriteria;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Users.TestObjects;
using Payments.Core.Users.Application.List;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Application;

public class ListUsersHandlerTests : UsersTestBase
{
    [Fact]
    public async Task FindWhenNoFiltersShouldReturnAllUsers()
    {

        FakeUserRepository repository = new();
        List<User> users = UserMother.RandomMultiple(2);
        foreach (User user in users)
        {
            await repository.Save(user);
        }
        ListUsersHandler handler = new(repository);


        ListUsersResponse response = await handler.Find(new List<Dictionary<string, string>>());


        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
        Assert.Equal(2, response.Total);
        Assert.Contains(response.Users, u => u.Id == users[0].Id.Value);
        Assert.Contains(response.Users, u => u.Id == users[1].Id.Value);
    }

    [Fact]
    public async Task FindWhenEmailFilterShouldReturnFilteredUsers()
    {

        FakeUserRepository repository = new();
        User user1 = UserMother.RandomWith(email: "john.wick@example.com");
        User user2 = UserMother.Random();
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "john" }
        };


        ListUsersResponse response = await handler.Find(filters);


        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user1.Id.Value, response.Users[0].Id);
    }

    [Fact]
    public async Task FindWhenFullNameFilterShouldReturnFilteredUsers()
    {

        FakeUserRepository repository = new();
        User user1 = UserMother.Random();
        User user2 = UserMother.RandomWith(email: "jane@example.com", fullName: "Jane Austen");
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "fullname", ["operator"] = "CONTAINS", ["value"] = "Jane" }
        };


        ListUsersResponse response = await handler.Find(filters);


        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user2.Id.Value, response.Users[0].Id);
    }

    [Fact]
    public async Task FindWhenBothFiltersShouldReturnFilteredUsers()
    {

        FakeUserRepository repository = new();
        User user1 = UserMother.RandomWith(email: "john@example.com", fullName: "John Smith");
        User user2 = UserMother.Random();
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "john" },
            new() { ["field"] = "fullname", ["operator"] = "CONTAINS", ["value"] = "John" }
        };


        ListUsersResponse response = await handler.Find(filters);


        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user1.Id.Value, response.Users[0].Id);
    }

    [Fact]
    public async Task FindWhenNoMatchingUsersShouldReturnEmptyList()
    {

        FakeUserRepository repository = new();
        User user1 = UserMother.Random();
        await repository.Save(user1);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "nonexistent" }
        };


        ListUsersResponse response = await handler.Find(filters);


        Assert.NotNull(response);
        Assert.Empty(response.Users);
        Assert.Equal(0, response.Total);
    }

    [Fact]
    public async Task FindWhenNotAllowedFieldShouldIgnoreFilter()
    {

        FakeUserRepository repository = new();
        User user1 = UserMother.RandomWith(email: "john.wick@example.com");
        User user2 = UserMother.Random();
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "john" },
            new() { ["field"] = "invalidField", ["operator"] = "CONTAINS", ["value"] = "someValue" }
        };


        ListUsersResponse response = await handler.Find(filters);


        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user1.Id.Value, response.Users[0].Id);
    }

    private sealed class FakeUserRepository : TestUserRepositoryBase
    {
        public List<Uuid> FindCalls { get; } = new();
        public List<Criteria> MatchingCalls { get; } = new();

        public override Task<User?> Find(Uuid id)
        {
            FindCalls.Add(id);
            return base.Find(id);
        }

        public override Task<IEnumerable<User>> Matching(Criteria criteria)
        {
            MatchingCalls.Add(criteria);

            IEnumerable<User> results = _users.Values;


            if (criteria.HasFilters() && criteria.Filters != null)
            {
                foreach (Filter filter in criteria.Filters.Values)
                {
                    if (filter.Field.Value.Equals("email", StringComparison.OrdinalIgnoreCase) && filter.Operator == FilterOperator.CONTAINS)
                    {
                        results = results.Where(u => u.Email.Value.Contains(filter.Value.Value, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (filter.Field.Value.Equals("fullname", StringComparison.OrdinalIgnoreCase) && filter.Operator == FilterOperator.CONTAINS)
                    {
                        results = results.Where(u => u.FullName.Value.Contains(filter.Value.Value, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }


            if (criteria.HasOrder() && criteria.Order?.OrderBy.Value == "CreatedAt")
            {
                results = criteria.Order.OrderType == OrderType.DESC
                    ? results.OrderByDescending(u => u.CreatedAt)
                    : results.OrderBy(u => u.CreatedAt);
            }


            if (criteria.Offset.HasValue)
            {
                results = results.Skip(criteria.Offset.Value);
            }

            if (criteria.Limit.HasValue)
            {
                results = results.Take(criteria.Limit.Value);
            }

            return Task.FromResult(results);
        }
    }
}
