using Archetype.Core.Shared.Domain;
using Archetype.Core.Tests.Shared.Domain;

using Bogus;

namespace Archetype.Core.Tests.Users.TestObjects;

public abstract class UsersTestBase
{
    protected static readonly Faker Faker = new();
    protected static readonly IHasher DefaultHasher = new FakeHasher();

    public static string GenerateValidPassword() => TestPasswordGenerator.GenerateValidPassword();
}
