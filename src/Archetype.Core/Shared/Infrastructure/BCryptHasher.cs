using Archetype.Core.Shared.Domain;

using BCryptAlgorithm = BCrypt.Net.BCrypt;

namespace Archetype.Core.Shared.Infrastructure;

public class BCryptHasher : IHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainText) => BCryptAlgorithm.HashPassword(plainText, WorkFactor);

    public bool Verify(string plainText, string textHash)
    {
        try
        {
            return BCryptAlgorithm.Verify(plainText, textHash);
        }
        catch
        {
            return false;
        }
    }
}
