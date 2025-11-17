using System.Net;
using System.Net.Http.Json;

using Archetype.Api.IntegrationTests.Support;
using Archetype.Core.Users.Domain;

namespace Archetype.Api.IntegrationTests.Endpoints.Auth;

public class LoginEndpointTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task LoginWithValidCredentialsShouldReturnTokenEnvelope()
    {
        (User user, string password) = await IntegrationTestData.CreateUser(Factory);
        LoginRequest request = new(user.Email.Value, password);

        HttpResponseMessage response = await Client.PostAsJsonAsync("/auth/login", request, SnakeCaseJson.Options);

        response.EnsureSuccessStatusCode();
        ApiSuccessEnvelope? payload = await response.Content.ReadFromJsonAsync<ApiSuccessEnvelope>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
    }

    [Fact]
    public async Task LoginWithInvalidPasswordShouldReturnUnauthorizedError()
    {
        (User user, string password) = await IntegrationTestData.CreateUser(Factory);
        string invalidPassword;
        do
        {
            invalidPassword = IntegrationTestData.GenerateValidPassword();
        }
        while (invalidPassword == password);

        LoginRequest request = new(user.Email.Value, invalidPassword);

        HttpResponseMessage response = await Client.PostAsJsonAsync("/auth/login", request, SnakeCaseJson.Options);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        ApiErrorEnvelope? payload = await response.Content.ReadFromJsonAsync<ApiErrorEnvelope>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.False(payload!.Success);
        Assert.Equal("Invalid credentials.", payload.Error.Message);
        Assert.Equal("INVALID_CREDENTIALS", payload.Error.Code);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
    }

    [Fact]
    public async Task LoginWithInvalidPayloadShouldReturnValidationErrors()
    {
        HttpResponseMessage response = await Client.PostAsJsonAsync("/auth/login", new { password = "Mono8210!" }, SnakeCaseJson.Options);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        ApiErrorEnvelope? payload = await response.Content.ReadFromJsonAsync<ApiErrorEnvelope>(SnakeCaseJson.Options);

        Assert.NotNull(payload);
        Assert.False(payload!.Success);
        Assert.Equal("Validation error.", payload.Error.Message);
        Assert.Equal("VALIDATION_ERROR", payload.Error.Code);
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.RequestId));
        Assert.False(string.IsNullOrWhiteSpace(payload.Meta.CorrelationId));
        Assert.Contains(payload.Error.Fields!, field => field.Name == "Email");
    }

    private sealed record LoginRequest(string Email, string Password);
    private sealed record ApiSuccessEnvelope(LoginResponse Data, ApiMeta Meta, bool Success);
    private sealed record LoginResponse(string Token);
    private sealed record ApiErrorEnvelope(ApiErrorBody Error, ApiMeta Meta, bool Success);
    private sealed record ApiErrorBody(string Code, string Message, string? Details, ApiFieldError[]? Fields, bool Retryable);
    private sealed record ApiFieldError(string Name, string Message);
    private sealed record ApiMeta(string RequestId, string? CorrelationId, ApiPagination? Pagination, string? UserId);
    private sealed record ApiPagination(int Page, int Size, long? Total, string? NextCursor);
}
