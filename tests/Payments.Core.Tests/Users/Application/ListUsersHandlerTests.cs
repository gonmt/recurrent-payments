using Bogus;

using Payments.Core.Shared.Domain.FiltersByCriteria;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Application.List;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Application;

public class ListUsersHandlerTests
{
    [Fact]
    public async Task FindWhenNoFiltersShouldReturnAllUsers()
    {
        FakeUserRepository repository = new();
        User user1 = CreateUser();
        User user2 = CreateUser();
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        ListUsersResponse response = await handler.Find(new List<Dictionary<string, string>>());

        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
        Assert.Equal(2, response.Total);
        Assert.Contains(response.Users, u => u.Id == user1.Id.Value);
        Assert.Contains(response.Users, u => u.Id == user2.Id.Value);
    }

    [Fact]
    public async Task FindWhenEmailFilterShouldReturnFilteredUsers()
    {
        FakeUserRepository repository = new();
        User user1 = CreateUser(email: "john.doe@example.com");
        User user2 = CreateUser(email: "jane.smith@example.com");
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["email"] = "john" }
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
        User user1 = CreateUser(fullName: "John Doe");
        User user2 = CreateUser(fullName: "Jane Smith");
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["fullName"] = "Jane" }
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
        User user1 = CreateUser(email: "john@example.com", fullName: "John Doe");
        User user2 = CreateUser(email: "jane@example.com", fullName: "Jane Smith");
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["email"] = "john", ["fullName"] = "John" }
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
        User user1 = CreateUser(email: "john@example.com", fullName: "John Doe");
        await repository.Save(user1);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["email"] = "nonexistent" }
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
        User user1 = CreateUser(email: "john@example.com", fullName: "John Doe");
        User user2 = CreateUser(email: "jane@example.com", fullName: "Jane Smith");
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["email"] = "john" },
            new() { ["invalidField"] = "someValue" }
        };
        ListUsersResponse response = await handler.Find(filters);

        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user1.Id.Value, response.Users[0].Id);
    }

    private static User CreateUser(string? email = null, string? fullName = null)
    {
        Faker faker = new();
        FakeHasher hasher = new();
        Uuid id = Uuid.New();
        EmailAddress emailAddress = new(email ?? faker.Internet.Email());
        UserFullName userFullName = new(fullName ?? faker.Name.FullName());
        UserPasswordHash password = UserPasswordHash.Create("Valid12!", hasher);

        return User.Create(id, emailAddress, userFullName, password);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly Dictionary<Uuid, User> _users = new();
        public List<Uuid> FindCalls { get; } = new();
        public List<Criteria> MatchingCalls { get; } = new();

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

        public Task<IEnumerable<User>> Matching(Criteria criteria)
        {
            MatchingCalls.Add(criteria);

            IEnumerable<User> results = _users.Values;

            // Apply simple filtering for testing purposes
            if (criteria.HasFilters() && criteria.Filters != null)
            {
                foreach (Filter filter in criteria.Filters.Values)
                {
                    if (filter.Field.Value == "Email" && filter.Operator == FilterOperator.CONTAINS)
                    {
                        results = results.Where(u => u.Email.Value.Contains(filter.Value.Value, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (filter.Field.Value == "FullName" && filter.Operator == FilterOperator.CONTAINS)
                    {
                        results = results.Where(u => u.FullName.Value.Contains(filter.Value.Value, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            // Apply ordering
            if (criteria.HasOrder() && criteria.Order?.OrderBy.Value == "CreatedAt")
            {
                results = criteria.Order.OrderType == OrderType.DESC
                    ? results.OrderByDescending(u => u.CreatedAt)
                    : results.OrderBy(u => u.CreatedAt);
            }

            // Apply pagination
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

        public Task Save(User user)
        {
            _users[user.Id] = user;
            return Task.CompletedTask;
        }
    }
}
