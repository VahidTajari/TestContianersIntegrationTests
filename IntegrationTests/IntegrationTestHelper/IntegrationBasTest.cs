using Xunit;

namespace IntegrationTests.IntegrationTestHelper;
public class IntegrationTestBase<T> : IClassFixture<CustomFactory<T>> where T : class
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomFactory<T> apiFactory)
    {
        Client = apiFactory.CreateClient();
    }
}
