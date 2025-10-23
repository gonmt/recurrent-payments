namespace Archetype.Core.Users.Application;

public record GetUserResponse(string Id, string Email, string FullName, string CreatedAt);
