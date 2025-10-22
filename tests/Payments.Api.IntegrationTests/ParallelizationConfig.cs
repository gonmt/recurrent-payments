namespace Payments.Api.IntegrationTests;

/// <summary>
/// Integration tests typically run sequentially to avoid database conflicts
/// </summary>
[CollectionDefinition("Integration Tests", DisableParallelization = true)]
public class IntegrationTestsGroup;
