using FluentAssertions;
using S1337.Services;
using Xunit;

namespace S1337.Test;

public class RequestUriBuilderTests
{
    [Theory]
    [InlineData("","1337.tech", "http://1337.tech/")]
    [InlineData("/", "1337.tech", "http://1337.tech/")]
    [InlineData("www.google.com", "1337.tech", "http://www.google.com/")]
    [InlineData("/////www.google.com", "1337.tech", "http://www.google.com/")]
    [InlineData("/fave.ico", "1337.tech", "http://1337.tech/fave.ico")]
    public void Test(string rowUrl,string domain,string expected)
    {
        var builder = new RequestUriBuilder();
        var result = builder.Build(rowUrl, domain);
        result.AbsoluteUri.Should().Be(expected);

    }
}