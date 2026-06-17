using System.Net;

namespace TechMoveTester;


public class ContractApiIntegrationTests
{
    private readonly HttpClient _client;

    public ContractApiIntegrationTests()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5289/")
        };
    }

    [Fact]
    public async Task GetContracts_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("api/contracts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}
