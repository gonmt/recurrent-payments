using System.Net;
using System.Net.Http.Json;

using Archetype.Api.IntegrationTests.Support;
using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Api.IntegrationTests.Endpoints.Users;

public class GetUserEndpointTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetUserWithExistingUserShouldReturnEnvelopeWithUser()
    {
        (Core.Users.Domain.User user, _) = await IntegrationTestData.CreateUser(Factory);

        HttpResponseMessage response = await Client.GetAsync($"/users/{user.Id.Value}");

        response.EnsureSuccessStatusCode();
        Envelope<GetUserResponse>? payload = await response.Content.ReadFromJsonAsync<Envelope<GetUserResponse>>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal(user.Id.Value, payload.Data!.Id);
        Assert.Equal(user.Email.Value, payload.Data.Email);
        Assert.Equal(user.FullName.Value, payload.Data.FullName);
    }

    [Fact]
    public async Task GetUserWithUnknownUserShouldReturnEnvelopeWithNullData()
    {
        string unknownId = Uuid.New().Value;
        HttpResponseMessage response = await Client.GetAsync($"/users/{unknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        ErrorEnvelope? payload = await response.Content.ReadFromJsonAsync<ErrorEnvelope>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.False(payload!.Success);
        Assert.Equal("User not found.", payload.Error.Message);
        Assert.Equal("USER_NOT_FOUND", payload.Error.Code);
    }

    private sealed record Envelope<T>(T? Data, ApiMeta Meta, bool Success);
    private sealed record ApiMeta(string RequestId, string? CorrelationId, ApiPagination? Pagination, string? UserId);
    private sealed record ApiPagination(int Page, int Size, long? Total, string? NextCursor);
    private sealed record GetUserResponse(string Id, string Email, string FullName, string CreatedAt);
    private sealed record ErrorEnvelope(ErrorBody Error, ApiMeta Meta, bool Success);
    private sealed record ErrorBody(string Code, string Message, string? Details, object? Fields, bool Retryable);
}
