using System.Net;

namespace TechMoveTester;


public class ClientApiIntegrationTests
{
    private readonly HttpClient _client;

    public ClientApiIntegrationTests()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5289/")
        };
    }

    [Fact]
    public async Task GetClients_ReturnsOk()
    {
        var response = await _client.GetAsync("api/clients");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}
