using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;

using Payments.Api.IntegrationTests.Support;
using Payments.Core.Users.Application;
using Payments.Core.Users.Domain;

namespace Payments.Api.IntegrationTests.Endpoints.Users;

public class GetUserEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;
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
    }

    [Fact]
    public async Task GetUserWithUnknownUserShouldReturnEnvelopeWithNullData()
    {
        string unknownUserId = Guid.CreateVersion7().ToString();

        HttpResponseMessage response = await _client.GetAsync($"/users/{unknownUserId}");

        response.EnsureSuccessStatusCode();
        ApiEnvelope<GetUserResponse?>? payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<GetUserResponse?>>();

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Null(payload.Data);
    }

    private sealed record ApiEnvelope<T>(T? Data, ApiMeta Meta, bool Success);
    private sealed record ApiMeta(string RequestId, ApiPagination? Pagination);
    private sealed record ApiPagination(int Page, int Size, long? Total, string? NextCursor);
}
