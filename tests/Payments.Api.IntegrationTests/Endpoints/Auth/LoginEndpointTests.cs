using System.Net;
using System.Net.Http.Json;

using Payments.Api.IntegrationTests.Support;
using Payments.Core.Users.Domain;

namespace Payments.Api.IntegrationTests.Endpoints.Auth;

public class LoginEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task LoginWithValidCredentialsShouldReturnTokenEnvelope()
    {
        (User user, string password) = await IntegrationTestData.CreateUser(factory);
        LoginRequest request = new(user.Email.Value, password);

        HttpResponseMessage response = await _client.PostAsJsonAsync("/auth/login", request);

        response.EnsureSuccessStatusCode();
        ApiSuccessEnvelope? payload = await response.Content.ReadFromJsonAsync<ApiSuccessEnvelope>();

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
    }

    [Fact]
    public async Task LoginWithInvalidPasswordShouldReturnUnauthorizedError()
    {
        (User user, string password) = await IntegrationTestData.CreateUser(factory);
        string invalidPassword;
        do
        {
            invalidPassword = IntegrationTestData.GenerateValidPassword();
        }
        while (invalidPassword == password);

        LoginRequest request = new(user.Email.Value, invalidPassword);

        HttpResponseMessage response = await _client.PostAsJsonAsync("/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        ApiErrorEnvelope? payload = await response.Content.ReadFromJsonAsync<ApiErrorEnvelope>();

        Assert.NotNull(payload);
        Assert.False(payload!.Success);
        Assert.Equal("Invalid credentials.", payload.Error.Message);
    }

    [Fact]
    public async Task LoginWithInvalidPayloadShouldReturnValidationErrors()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/auth/login", new { Password = "Mono8210!" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        ApiErrorEnvelope? payload = await response.Content.ReadFromJsonAsync<ApiErrorEnvelope>();

        Assert.NotNull(payload);
        Assert.False(payload!.Success);
        Assert.Equal("Validation error.", payload.Error.Message);
        Assert.Contains(payload.Error.Fields!, field => field.Name == "Email");
    }

    private sealed record LoginRequest(string Email, string Password);
    private sealed record ApiSuccessEnvelope(LoginResponse Data, ApiMeta Meta, bool Success);
    private sealed record LoginResponse(string Token);
    private sealed record ApiErrorEnvelope(ApiErrorBody Error, ApiMeta Meta, bool Success);
    private sealed record ApiErrorBody(string Code, string Message, string? Details, ApiFieldError[]? Fields, bool Retryable);
    private sealed record ApiFieldError(string Name, string Message);
    private sealed record ApiMeta(string RequestId, ApiPagination? Pagination);
    private sealed record ApiPagination(int Page, int Size, long? Total, string? NextCursor);
}
