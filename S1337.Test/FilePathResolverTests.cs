using FluentAssertions;
using S1337.Services;
using Xunit;

namespace S1337.Test;

public class FilePathResolverTests
{

    [Theory]
    [InlineData("http://1337.tech", @"D:\site", @"text/html", @"D:\site\index.html")]
    [InlineData("http://1337.tech", @"D:\site", "", @"D:\site\index.html")]
    [InlineData("http://1337.tech/images/img1.jpg", @"D:\site", "", @"D:\site\images\img1.jpg")]
    [InlineData("http://1337.tech/images/company", @"D:\site", @"text/html", @"D:\site\images\company.html")]
    [InlineData("http://1337.tech/images/company", @"D:\site", "", @"D:\site\images\company")]
    public void ResolvePathMethodMustReturnTheRelatedOnDiskPath(string url, string baseFolder, string? mimeType,string expected)
    {
        var resolver = new FilePathResolver();
        var result = resolver.ResolvePath(url, baseFolder, mimeType);
        result.Should().Be(expected);
    }
}