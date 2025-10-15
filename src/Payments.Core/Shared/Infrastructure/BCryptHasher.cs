using Payments.Core.Shared.Domain;

namespace Payments.Core.Shared.Infrastructure;

using BCrypt.Net;

public class BCryptHasher : IHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainText) => BCrypt.HashPassword(plainText, WorkFactor);

    public bool Verify(string plainText, string textHash)
    {
        try
        {
            return BCrypt.Verify(plainText, textHash);
        }
        catch
        {
            return false;
        }
    }
}
