using StarBlog.IntegrationTests.Infrastructure;

namespace StarBlog.IntegrationTests.Endpoints;

public sealed class RobotsTxtTests : IClassFixture<StarBlogWebApplicationFactory> {
    private readonly HttpClient _client;

    public RobotsTxtTests(StarBlogWebApplicationFactory factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_RobotsTxt_ReturnsSitemap() {
        var response = await _client.GetAsync("/robots.txt");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Sitemap:", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("/sitemap.xml", body, StringComparison.OrdinalIgnoreCase);
    }
}

