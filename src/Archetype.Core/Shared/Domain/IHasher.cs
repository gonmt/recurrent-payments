namespace Archetype.Core.Shared.Domain;

public interface IHasher
{
    string Hash(string plainText);
    bool Verify(string plainText, string textHash);
}
