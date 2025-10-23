namespace Archetype.Core.Tests;

[CollectionDefinition("Parallel Tests", DisableParallelization = false)]
public class ParallelTestsGroup;

[CollectionDefinition("Sequential Tests", DisableParallelization = true)]
public class SequentialTestsGroup;
