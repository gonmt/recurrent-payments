namespace Payments.Core.Auth.Domain;

public interface ITokenProvider
{
    string Generate(string userId, string email, string fullName);
}
