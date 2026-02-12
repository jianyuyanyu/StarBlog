using System.Text.Json;
using StarBlog.IntegrationTests.Infrastructure;

namespace StarBlog.IntegrationTests.Endpoints;

public sealed class ThemeApiTests : IClassFixture<StarBlogWebApplicationFactory> {
    private readonly HttpClient _client;

    public ThemeApiTests(StarBlogWebApplicationFactory factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Themes_ReturnsArray() {
        var response = await _client.GetAsync("/Api/Theme");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);

        var data = doc.RootElement.GetProperty("data");
        Assert.Equal(JsonValueKind.Array, data.ValueKind);
        Assert.True(data.GetArrayLength() >= 1);
    }
}
