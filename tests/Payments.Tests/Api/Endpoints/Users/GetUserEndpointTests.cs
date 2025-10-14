using System;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Payments.Core.Users.Application;

namespace Payments.Tests.Api.Endpoints.Users;

public class GetUserEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GetUserEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_WithExistingUser_ShouldReturnEnvelopeWithUser()
    {
        const string existingUserId = "0199db05-c460-72ce-891d-5892875f9663";

        var response = await _client.GetAsync($"/users/{existingUserId}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<GetUserResponse?>>();

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal(existingUserId, payload.Data!.Id);
        Assert.Equal("gon@gmail.com", payload.Data.Email);
        Assert.Equal("Gonzalo Torres", payload.Data.FullName);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
    }

    [Fact]
    public async Task GetUser_WithUnknownUser_ShouldReturnEnvelopeWithNullData()
    {
        var unknownUserId = Guid.CreateVersion7().ToString();

        var response = await _client.GetAsync($"/users/{unknownUserId}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<GetUserResponse?>>();

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Null(payload.Data);
    }

    private sealed record ApiEnvelope<T>(T? Data, ApiMeta Meta, bool Success);
    private sealed record ApiMeta(string RequestId, ApiPagination? Pagination);
    private sealed record ApiPagination(int Page, int Size, long? Total, string? NextCursor);
}
