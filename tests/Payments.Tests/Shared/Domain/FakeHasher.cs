using System;
using System.Collections.Generic;

using Payments.Core.Shared.Domain;

namespace Payments.Tests.Shared.Domain;

public sealed class FakeHasher : IHasher
{
    private readonly Func<string, string>? _hashFunc;
    private readonly Func<string, string, bool>? _verifyFunc;

    public List<(string Plain, string Result)> HashCalls { get; } = new();
    public List<(string Plain, string Hash)> VerifyCalls { get; } = new();

    public FakeHasher(
        Func<string, string>? hashFunc = null,
        Func<string, string, bool>? verifyFunc = null)
    {
        _hashFunc = hashFunc;
        _verifyFunc = verifyFunc;
    }

    public string Hash(string plainText)
    {
        var result = _hashFunc?.Invoke(plainText) ?? $"{plainText}_HASH";
        HashCalls.Add((plainText, result));
        return result;
    }

    public bool Verify(string plainText, string textHash)
    {
        var result = _verifyFunc?.Invoke(plainText, textHash) ?? textHash == $"{plainText}_HASH";
        VerifyCalls.Add((plainText, textHash));
        return result;
    }
}
