using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using S1337.Core;
using S1337.Services;
using Xunit;

namespace S1337.Test;

public class ScannerTests
{
    [Fact]
    public async Task ScanMethodMustFindAnyHrefTagUrlInTheSpecifiedUrl()
    {
        var url = "http://www.test1.com";
        var content1 = @"<a href=""http://www.test1.com/a"" ></a>
            <a href=""http://www.test1.com/a.jpg"" ></a>
            <a href=""https://www.c.com"" ></a>
            <a href=""http://www.d.com""></a>
            <a href=""http://www.test2.com""></a>";

        var content2 = @"
            <a href=""http://www.e.com"" ></a>
            <a href=""http://www.test1.com/#a"" ></a 
            <a href=""https://www.g.com"" ></a>";

        var handler = new Mock<HttpMessageHandler>();
        var client = handler.CreateClient();

        handler.SetupRequest(HttpMethod.Get, url)
            .ReturnsResponse(content1, "text/html");
        handler.SetupRequest(HttpMethod.Get, "http://www.test2.com")
            .ReturnsResponse(content2, "text/html");

        var result1 = new List<string>
        {
            "http://www.test1.com/a",
            "http://www.test1.com/a.jpg",
            "https://www.c.com",
            "http://www.d.com",
            "http://www.test2.com"
        };
        var result2 = new List<string>
        {
            "http://www.e.com",
            "http://www.test1.com/#a",
            "https://www.g.com"
        };
        var mockUrlFinder = new Mock<IUrlFinder>();
        mockUrlFinder.Setup(f => f.FindUrls(content1)).Returns(result1);
        mockUrlFinder.Setup(f => f.FindUrls(content2)).Returns(result2);

        var requestUriBuilder = new RequestUriBuilder();

        var scanner = new Scanner(mockUrlFinder.Object, client,requestUriBuilder);
        var result = new List<ScanResult>();
        await foreach (var u in scanner.Scan(url))
        {
            result.Add(u);
        }
      
        result.Should()
            .NotBeEmpty()
            .And.HaveCount(3);

    }
}