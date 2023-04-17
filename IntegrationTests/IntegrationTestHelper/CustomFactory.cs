using DotNet.Testcontainers.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace IntegrationTests.IntegrationTestHelper;

#pragma warning disable CA2213
public class CustomFactory<T> : WebApplicationFactory<T>, IAsyncLifetime where T : class
{
    private readonly WireMockServer _server;

    public CustomFactory()
    {
        _server = WireMockServer.Start();

    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri(_server.Urls[0]);

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IHttpClientFactory>(new MockHttpClientFactory(httpClient));

        }).ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["connectionStrings:DBContext"] = TestContainerHelper.GetConnectionString,
                    ["JaegerConfig:IsEnabled"] = "false",
                    ["CacheConfig:IsEnabled"] = "false"
                });
        }).UseEnvironment("Development");
    }

    public async Task InitializeAsync()
    {
        if (TestContainerHelper.DbContainer != null)
        {
            await TestContainerHelper.DbContainer.StartAsync();
            TestContainerHelper.SeedDatabase();
        }
    }

    public new async Task DisposeAsync()
    {

        await Task.Run(_server.Dispose);
    }

    public void ConfigureApiResponse(params ApiResponse[] apiResponses)
    {
        foreach (var apiResponse in apiResponses)
        {
            _server.Given(Request.Create().WithPath(apiResponse.Path).UsingMethod(apiResponse.Method)).RespondWith(Response.Create()
                .WithStatusCode(apiResponse.ResponseStatusCode).WithBody(apiResponse.ResponseJson));
        }
    }
}
