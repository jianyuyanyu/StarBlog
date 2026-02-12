using System.Net;
using StarBlog.Infrastructure.Ip;

namespace StarBlog.UnitTests.Infrastructure;

public sealed class FakeIpSearcherTests {
    [Fact]
    public void Search_AllOverloads_ReturnSameValue() {
        using var searcher = new FakeIpSearcher();

        var a = searcher.Search("127.0.0.1");
        var b = searcher.Search(IPAddress.Loopback);
        var c = searcher.Search(2130706433u);

        Assert.Equal("0|0|0|0|0", a);
        Assert.Equal(a, b);
        Assert.Equal(a, c);
        Assert.Equal(0, searcher.IoCount);
    }
}

