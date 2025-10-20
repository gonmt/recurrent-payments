namespace Payments.Core.Users.Application.List;

public record ListUsersResponse(List<UserSummaryResponse> Users, int Total);

public record UserSummaryResponse(string Id, string Email, string FullName, string CreatedAt);
