using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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

         async IAsyncEnumerable<string> Result1()
        {
            yield return "http://www.test1.com/a";
           yield return "http://www.test1.com/a.jpg";
           yield return "https://www.c.com";
           yield return "http://www.d.com";
           yield return "http://www.test2.com";
        };


         async IAsyncEnumerable<string> Result2()
         {
            yield return "http://www.e.com";
            yield return "http://www.test1.com/#a";
            yield return "https://www.g.com";
         };
        var mockUrlFinder = new Mock<IUrlFinder>();
        mockUrlFinder.Setup( f => f.FindUrls(content1,url))
            .Returns(Result1);
        mockUrlFinder.Setup(f => f.FindUrls(content2,url)).Returns(Result2);
        var moqLogger = new Mock<ILogger<Scanner>>();
        var scanner = new Scanner(mockUrlFinder.Object, client,moqLogger.Object);
        var result = await scanner.Scan(url).ToListAsync();

        result.Should()
            .NotBeEmpty()
            .And.HaveCount(3);

    }
}