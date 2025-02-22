namespace Uppbeat.Api.IntegrationTests;

[CollectionDefinition(nameof(SequentialTestsCollection), DisableParallelization = true)]
public class SequentialTestsCollection
{
    // This class has no code, and is never created. Its purpose is simply to make sure data dependant tests don't run in parallel.
    // This could always be looked at if the runtime becomes excessive but keeps the tests fairly clean.
}
