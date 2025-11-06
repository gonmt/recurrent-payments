namespace Archetype.Api.IntegrationTests.Support;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal sealed class IntegrationTestFactAttribute : FactAttribute
{
    public IntegrationTestFactAttribute()
    {
        if (!IntegrationTestEnvironment.EnsureInitialized())
        {
            Exception? error = IntegrationTestEnvironment.Error;
            Skip = error != null
                ? $"Integration test skipped: unable to configure database ({error.GetType().Name}: {error.Message})."
                : "Integration test skipped: database connection string could not be determined.";
        }
    }
}
