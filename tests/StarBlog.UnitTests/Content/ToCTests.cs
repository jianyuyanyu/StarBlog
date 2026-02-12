using StarBlog.Content.Extensions.Markdown;
using StarBlog.Data.Models;

namespace StarBlog.UnitTests.Content;

public sealed class ToCTests {
    [Fact]
    public void ExtractToc_ContentNull_ReturnsNull() {
        var post = new Post { Id = "p1", Title = "t1", Content = null };

        var toc = post.ExtractToc();

        Assert.Null(toc);
    }

    [Fact]
    public void ExtractToc_SingleHeading_ReturnsSingleNode() {
        var post = new Post {
            Id = "p1",
            Title = "t1",
            Content = "# Hello\n\nBody"
        };

        var toc = post.ExtractToc();

        Assert.NotNull(toc);
        var node = Assert.Single(toc);
        Assert.Equal("Hello", node.Text);
        Assert.False(string.IsNullOrWhiteSpace(node.Href));
        Assert.StartsWith("#", node.Href);
        Assert.Null(node.Nodes);
    }

    [Fact]
    public void ExtractToc_NestedHeadings_BuildsTree() {
        var post = new Post {
            Id = "p1",
            Title = "t1",
            Content = """
                      # A
                      ## B
                      ### C
                      ## D
                      # E
                      """
        };

        var toc = post.ExtractToc();

        Assert.NotNull(toc);
        Assert.Equal(2, toc.Count);

        Assert.Equal("A", toc[0].Text);
        Assert.NotNull(toc[0].Nodes);
        Assert.Equal(2, toc[0].Nodes!.Count);

        Assert.Equal("B", toc[0].Nodes![0].Text);
        Assert.NotNull(toc[0].Nodes![0].Nodes);
        Assert.Single(toc[0].Nodes![0].Nodes!);
        Assert.Equal("C", toc[0].Nodes![0].Nodes![0].Text);

        Assert.Equal("D", toc[0].Nodes![1].Text);
        Assert.Null(toc[0].Nodes![1].Nodes);

        Assert.Equal("E", toc[1].Text);
        Assert.Null(toc[1].Nodes);
    }
}

