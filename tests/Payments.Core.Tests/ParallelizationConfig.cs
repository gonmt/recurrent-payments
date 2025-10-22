namespace Payments.Core.Tests;

/// <summary>
/// Collection for tests that can run in parallel
/// </summary>
[CollectionDefinition("Parallel Tests", DisableParallelization = false)]
public class ParallelTestsGroup;

/// <summary>
/// Collection for tests that must run sequentially (if needed)
/// </summary>
[CollectionDefinition("Sequential Tests", DisableParallelization = true)]
public class SequentialTestsGroup;
