using System.Net;

namespace TechMoveTester;

public class ServiceRequestApiIntegrationTests
{
    private readonly HttpClient _client;

    public ServiceRequestApiIntegrationTests()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5289/")
        };
    }

    [Fact]
    public async Task GetServiceRequests_ReturnsOk()
    {
        var response = await _client.GetAsync("api/servicerequests");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}
