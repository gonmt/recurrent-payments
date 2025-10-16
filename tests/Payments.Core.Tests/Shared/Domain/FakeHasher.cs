using Payments.Core.Shared.Domain;

namespace Payments.Core.Tests.Shared.Domain;

public sealed class FakeHasher(
    Func<string, string>? hashFunc = null,
    Func<string, string, bool>? verifyFunc = null) : IHasher
{
    private readonly Func<string, string>? _hashFunc = hashFunc;
    private readonly Func<string, string, bool>? _verifyFunc = verifyFunc;

    public List<(string Plain, string Result)> HashCalls { get; } = new();
    public List<(string Plain, string Hash)> VerifyCalls { get; } = new();

    public string Hash(string plainText)
    {
        string result = _hashFunc?.Invoke(plainText) ?? $"{plainText}_HASH";
        HashCalls.Add((plainText, result));
        return result;
    }

    public bool Verify(string plainText, string textHash)
    {
        bool result = _verifyFunc?.Invoke(plainText, textHash) ?? textHash == $"{plainText}_HASH";
        VerifyCalls.Add((plainText, textHash));
        return result;
    }
}
