using StarBlog.Web.Extensions;

namespace StarBlog.UnitTests.Web;

public sealed class ConfigureSwaggerTests {
    [Fact]
    public void Groups_NameUnique() {
        var names = ConfigureSwagger.Groups.Select(g => g.Name).ToList();

        Assert.Equal(names.Count, names.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Groups_MatchesApiGroupConstants() {
        var expected = new[] {
            ApiGroups.Admin,
            ApiGroups.Auth,
            ApiGroups.Blog,
            ApiGroups.Comment,
            ApiGroups.Common,
            ApiGroups.Link,
            ApiGroups.Photo,
            ApiGroups.Test
        };

        var actual = ConfigureSwagger.Groups.Select(g => g.Name).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        var expectedOrdered = expected.OrderBy(x => x, StringComparer.Ordinal).ToArray();

        Assert.Equal(expectedOrdered, actual);
    }
}

