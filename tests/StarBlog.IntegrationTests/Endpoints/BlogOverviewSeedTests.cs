using System.Text.Json;
using StarBlog.IntegrationTests.Infrastructure;
using StarBlog.Testing;

namespace StarBlog.IntegrationTests.Endpoints;

public sealed class BlogOverviewSeedTests {
    [Fact]
    public async Task Get_BlogOverview_AfterSeeding_ReturnsCounts() {
        using var factory = new StarBlogWebApplicationFactory();

        await TestDatabaseSeeder.EnsureEfCoreDatabaseAsync(factory.Services);
        await TestDatabaseSeeder.SeedMinimalBlogDataAsync(factory.Services);

        var client = factory.CreateClient();
        var response = await client.GetAsync("/Api/Blog/Overview");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        var data = doc.RootElement.GetProperty("data");
        var postsCount = data.GetProperty("postsCount").GetInt32();
        var categoriesCount = data.GetProperty("categoriesCount").GetInt32();

        Assert.True(postsCount >= 1);
        Assert.True(categoriesCount >= 1);
    }
}

