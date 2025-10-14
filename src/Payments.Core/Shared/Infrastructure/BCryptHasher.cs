using Payments.Core.Shared.Domain;

namespace Payments.Core.Shared.Infrastructure;

using BCrypt.Net;

public class BCryptHasher : IHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainPassword)
    {
        return BCrypt.HashPassword(plainPassword, WorkFactor);
    }

    public bool Verify(string plainPassword, string hashedPassword)
    {
        try
        {
            return BCrypt.Verify(plainPassword, hashedPassword);
        }
        catch
        {
            return false;
        }
    }
}
