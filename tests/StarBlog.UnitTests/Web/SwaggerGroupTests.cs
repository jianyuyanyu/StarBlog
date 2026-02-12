using StarBlog.Web.Models;

namespace StarBlog.UnitTests.Web;

public sealed class SwaggerGroupTests {
    [Fact]
    public void ToOpenApiInfo_TitleAndDescriptionNull_DefaultToName() {
        var group = new SwaggerGroup("blog");

        var info = group.ToOpenApiInfo();

        Assert.Equal("blog", info.Title);
        Assert.Equal("blog", info.Description);
        Assert.Equal("1.0", info.Version);
    }

    [Fact]
    public void ToOpenApiInfo_UsesProvidedVersion() {
        var group = new SwaggerGroup("blog", "Blog APIs", "desc");

        var info = group.ToOpenApiInfo("2.0");

        Assert.Equal("Blog APIs", info.Title);
        Assert.Equal("desc", info.Description);
        Assert.Equal("2.0", info.Version);
    }
}

