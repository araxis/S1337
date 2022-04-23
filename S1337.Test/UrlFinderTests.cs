using FluentAssertions;
using S1337.Services;
using Xunit;

namespace S1337.Test;

public class UrlFinderTests
{
    [Fact]
    public void FindUrlsMethodMustFindAnyHrefTagUrls()
    {

        var content = @"<a href=""www.example.com/page.php?id=xxxx&name=yyyy"" ></a>
            < a href = ""http://www.example.com/page.php?id=xxxx&name=yyyy"" ></ a >
            < a href = ""https://www.example.com/page.php?id=xxxx&name=yyyy"" ></ a >
            < a href = ""www.example.com/page.php/404""></a>
            < a href = ""images/site/all""></a>";


        var finder = new UrlFinder();

        var result = finder.FindUrls(content);
        result.Should().NotBeEmpty()
            .And.HaveCount(5)
            .And.Contain(p => p == "https://www.example.com/page.php?id=xxxx&name=yyyy")
            .And.Contain(p => p == "images/site/all");
    }
}