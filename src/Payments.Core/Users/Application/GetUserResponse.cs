namespace Payments.Core.Users.Application;

public record class GetUserResponse(string Id, string Email, string FullName, string CreatedAt);
