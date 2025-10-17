namespace Payments.Core.Users.Application;

public sealed record AuthenticateUserResponse(string Id, string Email, string FullName);
