namespace Payments.Core.Tests;

// Collection for tests that can run in parallel
[CollectionDefinition("Parallel Tests", DisableParallelization = false)]
public class ParallelTestsGroup { }

// Collection for tests that must run sequentially (if needed)
[CollectionDefinition("Sequential Tests", DisableParallelization = true)]
public class SequentialTestsGroup { }
