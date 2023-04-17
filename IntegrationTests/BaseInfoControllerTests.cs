using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using System.Collections.Generic;
using IntegrationTests.IntegrationTestHelper;
using ApiResponse = IntegrationTests.IntegrationTestHelper.ApiResponse;

namespace Indra.Card.IntegrationTests;
public class BaseInfoControllerTests : IntegrationTestBase<Program>
{
    public BaseInfoControllerTests(CustomFactory<Program> apiFactory) : base(apiFactory)
    {
        apiFactory.ConfigureApiResponse(new ApiResponse("/wallet/api/v2/provinces", "GET", 200, "{\"Body\":[{\"Code\":\"1\",\"Name\":\"تهران\"},{\"Code\":\"2\",\"Name\":\"قم\"}]}"));
    }

    [Fact]
    public async Task BaseInfo_Province_Should_Return_Ok_Status_Code()
    {
        // Arrange
      
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "WeatherForecast");
      
        // Act
        var response = await Client.SendAsync(httpRequestMessage);


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

      
    }
}


