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
        // Arrange - Usando Object Mother para crear múltiples usuarios
        FakeUserRepository repository = new();
        List<User> users = UserMother.RandomMultiple(2);
        foreach (User user in users)
        {
            await repository.Save(user);
        }
        ListUsersHandler handler = new(repository);

        // Act
        ListUsersResponse response = await handler.Find(new List<Dictionary<string, string>>());

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
        Assert.Equal(2, response.Total);
        Assert.Contains(response.Users, u => u.Id == users[0].Id.Value);
        Assert.Contains(response.Users, u => u.Id == users[1].Id.Value);
    }

    [Fact]
    public async Task FindWhenEmailFilterShouldReturnFilteredUsers()
    {
        // Arrange - Usando Object Mother para usuarios específicos para filtrado
        FakeUserRepository repository = new();
        User user1 = UserMother.RandomWith(email: "john.wick@example.com"); // email contiene "john"
        User user2 = UserMother.Random(); // email aleatorio no contiene "john"
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "john" }
        };

        // Act
        ListUsersResponse response = await handler.Find(filters);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user1.Id.Value, response.Users[0].Id);
    }

    [Fact]
    public async Task FindWhenFullNameFilterShouldReturnFilteredUsers()
    {
        // Arrange - Usando Object Mother para usuarios específicos para filtrado por nombre
        FakeUserRepository repository = new();
        User user1 = UserMother.Random(); // nombre aleatorio no contiene "Jane"
        User user2 = UserMother.RandomWith(email: "jane@example.com", fullName: "Jane Austen"); // nombre contiene "Jane"
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "fullname", ["operator"] = "CONTAINS", ["value"] = "Jane" }
        };

        // Act
        ListUsersResponse response = await handler.Find(filters);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user2.Id.Value, response.Users[0].Id);
    }

    [Fact]
    public async Task FindWhenBothFiltersShouldReturnFilteredUsers()
    {
        // Arrange - Usando Object Mother para filtrado combinado
        FakeUserRepository repository = new();
        User user1 = UserMother.RandomWith(email: "john@example.com", fullName: "John Smith"); // email y nombre contienen "john"
        User user2 = UserMother.Random(); // email y nombre no contienen "john"
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "john" },
            new() { ["field"] = "fullname", ["operator"] = "CONTAINS", ["value"] = "John" }
        };

        // Act
        ListUsersResponse response = await handler.Find(filters);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Users);
        Assert.Equal(1, response.Total);
        Assert.Equal(user1.Id.Value, response.Users[0].Id);
    }

    [Fact]
    public async Task FindWhenNoMatchingUsersShouldReturnEmptyList()
    {
        // Arrange - Usando Object Mother para usuario que no coincide con el filtro
        FakeUserRepository repository = new();
        User user1 = UserMother.Random(); // email aleatorio no contiene "nonexistent"
        await repository.Save(user1);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "nonexistent" }
        };

        // Act
        ListUsersResponse response = await handler.Find(filters);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Users);
        Assert.Equal(0, response.Total);
    }

    [Fact]
    public async Task FindWhenNotAllowedFieldShouldIgnoreFilter()
    {
        // Arrange - Usando Object Mother para test de filtro inválido
        FakeUserRepository repository = new();
        User user1 = UserMother.RandomWith(email: "john.wick@example.com"); // email contiene "john"
        User user2 = UserMother.Random(); // email aleatorio no contiene "john"
        await repository.Save(user1);
        await repository.Save(user2);
        ListUsersHandler handler = new(repository);

        List<Dictionary<string, string>> filters = new()
        {
            new() { ["field"] = "email", ["operator"] = "CONTAINS", ["value"] = "john" },
            new() { ["field"] = "invalidField", ["operator"] = "CONTAINS", ["value"] = "someValue" }
        };

        // Act
        ListUsersResponse response = await handler.Find(filters);

        // Assert
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

            // Apply simple filtering for testing purposes
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
    }
}
