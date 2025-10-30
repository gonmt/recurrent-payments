using System.Net.Http.Json;

using Archetype.Api.IntegrationTests.Support;
using Archetype.Core.Users.Application;
using Archetype.Core.Users.Domain;

namespace Archetype.Api.IntegrationTests.Endpoints.Users;

public class GetUserEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetUserWithExistingUserShouldReturnEnvelopeWithUser()
    {
        (User User, string Password) userData = await IntegrationTestData.CreateUser(_factory);
        User user = userData.User;

        HttpResponseMessage response = await _client.GetAsync($"/users/{user.Id.Value}");

        response.EnsureSuccessStatusCode();
        ApiEnvelope<GetUserResponse?>? payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<GetUserResponse?>>();

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal(user.Id.Value, payload.Data!.Id);
        Assert.Equal(user.Email.Value, payload.Data.Email);
        Assert.Equal(user.FullName.Value, payload.Data.FullName);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
    }

    [Fact]
    public async Task GetUserWithUnknownUserShouldReturnEnvelopeWithNullData()
    {
        string unknownUserId = Guid.CreateVersion7().ToString();

        HttpResponseMessage response = await _client.GetAsync($"/users/{unknownUserId}");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        ApiErrorEnvelope? payload = await response.Content.ReadFromJsonAsync<ApiErrorEnvelope>();

        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("USER_NOT_FOUND", payload.Error.Code);
        Assert.Equal("User not found.", payload.Error.Message);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
    }

    private sealed record ApiEnvelope<T>(T? Data, ApiMeta Meta, bool Success);
    private sealed record ApiErrorEnvelope(ApiErrorBody Error, ApiMeta Meta, bool Success);
    private sealed record ApiErrorBody(string Code, string Message, string? Details, ApiFieldError[]? Fields, bool Retryable);
    private sealed record ApiFieldError(string Name, string Message);
    private sealed record ApiMeta(string RequestId, string? CorrelationId, ApiPagination? Pagination, string? UserId);
    private sealed record ApiPagination(int Page, int Size, long? Total, string? NextCursor);
}
