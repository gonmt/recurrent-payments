using System.Net.Http.Json;

using Archetype.Api.IntegrationTests.Support;

namespace Archetype.Api.IntegrationTests.Endpoints.Users;

public class UsersListEndpointTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [IntegrationTestFact]
    public async Task ListUsersWithoutPaginationReturnsUsers()
    {
        (Core.Users.Domain.User createdUser, _) = await IntegrationTestData.CreateUser(Factory);

        HttpResponseMessage response = await Client.GetAsync("/users");

        response.EnsureSuccessStatusCode();
        ApiEnvelope<ListUsersResponse>? payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<ListUsersResponse>>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.NotEmpty(payload.Data!.Users);
        Assert.True(payload.Data.Total >= payload.Data.Users.Count);
        Assert.Contains(payload.Data.Users, u => u.Id == createdUser.Id.Value);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
        Assert.Null(payload.Meta.Pagination);
    }

    [IntegrationTestFact]
    public async Task ListUsersWithPaginationParametersReturnsPaginationMetadata()
    {
        for (int i = 0; i < 6; i++)
        {
            _ = await IntegrationTestData.CreateUser(Factory);
        }

        HttpResponseMessage response = await Client.GetAsync("/users?limit=5&offset=5");

        response.EnsureSuccessStatusCode();
        ApiEnvelope<ListUsersResponse>? payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<ListUsersResponse>>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
        Assert.NotNull(payload.Meta.Pagination);
        Assert.Equal(2, payload.Meta.Pagination!.Page);
        Assert.Equal(5, payload.Meta.Pagination.Size);
        Assert.True(payload.Data!.Users.Count <= 5);
    }

    [IntegrationTestFact]
    public async Task ListUsersWithEmailFilterReturnsMatchingUser()
    {
        (Core.Users.Domain.User User, string _) userData = await IntegrationTestData.CreateUser(Factory);
        string targetEmail = userData.User.Email.Value;

        string query = $"/users?email-eq={Uri.EscapeDataString(targetEmail)}";
        HttpResponseMessage response = await Client.GetAsync(query);

        response.EnsureSuccessStatusCode();
        ApiEnvelope<ListUsersResponse>? payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<ListUsersResponse>>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal(1, payload.Data!.Total);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
        UserSummaryResponse user = Assert.Single(payload.Data.Users);
        Assert.Equal(targetEmail, user.Email);
    }

    [IntegrationTestFact]
    public async Task ListUsersWithEmailContainsFilterReturnsMatchingUser()
    {
        (Core.Users.Domain.User User, string _) userData = await IntegrationTestData.CreateUser(Factory);
        string targetEmail = userData.User.Email.Value;
        int partialLength = Math.Max(1, Math.Min(5, targetEmail.Length - 1));
        string partialEmail = targetEmail[..partialLength];

        string query = $"/users?email-contains={Uri.EscapeDataString(partialEmail)}";
        HttpResponseMessage response = await Client.GetAsync(query);

        response.EnsureSuccessStatusCode();
        ApiEnvelope<ListUsersResponse>? payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<ListUsersResponse>>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.True(payload.Data!.Total >= 1);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
        Assert.Contains(payload.Data.Users, user => user.Email == targetEmail);
    }

    [IntegrationTestFact]
    public async Task ListUsersWithEmailNotContainsFilterExcludesMatchingUser()
    {
        (Core.Users.Domain.User User, string _) userData = await IntegrationTestData.CreateUser(Factory);
        string targetEmail = userData.User.Email.Value;
        string partial = targetEmail[..Math.Max(1, Math.Min(5, targetEmail.Length - 1))];

        string query = $"/users?email-not-contains={Uri.EscapeDataString(partial)}";
        HttpResponseMessage response = await Client.GetAsync(query);

        response.EnsureSuccessStatusCode();
        ApiEnvelope<ListUsersResponse>? payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<ListUsersResponse>>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.DoesNotContain(payload.Data!.Users, user => user.Email == targetEmail);
    }

    private sealed record ApiEnvelope<T>(T Data, ApiMeta Meta, bool Success);
    private sealed record ApiMeta(string RequestId, string? CorrelationId, ApiPagination? Pagination, string? UserId);
    private sealed record ApiPagination(int Page, int Size, long? Total, string? NextCursor);
    private sealed record ListUsersResponse(List<UserSummaryResponse> Users, int Total);
    private sealed record UserSummaryResponse(string Id, string Email, string FullName, string CreatedAt);
}
