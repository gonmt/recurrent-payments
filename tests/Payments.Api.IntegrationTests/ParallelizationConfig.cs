namespace Payments.Api.IntegrationTests;

// Integration tests typically run sequentially to avoid database conflicts
[CollectionDefinition("Integration Tests", DisableParallelization = true)]
public class IntegrationTestsGroup { }
