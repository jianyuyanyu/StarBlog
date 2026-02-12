using System.Text.Json;
using StarBlog.IntegrationTests.Infrastructure;

namespace StarBlog.IntegrationTests.Endpoints;

public sealed class SwaggerJsonTests : IClassFixture<StarBlogWebApplicationFactory> {
    private readonly HttpClient _client;

    public SwaggerJsonTests(StarBlogWebApplicationFactory factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_SwaggerBlogJson_ReturnsOpenApiInfo() {
        var response = await _client.GetAsync("/swagger/blog/swagger.json");

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var info = doc.RootElement.GetProperty("info");
        var title = info.GetProperty("title").GetString();

        Assert.Equal("Blog APIs", title);
    }
}

