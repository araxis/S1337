using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using S1337.Services;
using Xunit;

namespace S1337.Test;

public class UrlFinderTests
{
 
   

    [Fact]
    public async Task  FindUrlsMethodMustFindAnyUrlsAndReturnsAbsoluteUrls()
    {


        var content = @"<a href=""www.example.com/page.php?id=xxxx&name=yyyy"" ></a>
            <img src=""/site.svg"" srcSet=""a/ site/a.jpg w400,b/d/global.png w4""/>
            <script src=""/_next/static/chunks/a.js""></script>
            <a href=""http://www.example.com/page.php?id=xxxx&name=yyyy""></a>
            <a href=""https://www.example.com/page1.php?id=xxxx&name=yyyy""></a>
            <a href=""a/page.svg""></a>
            <link href=""css / app.css"" rel=""stylesheet"" />
            <a href=""images/site/all""></a>";


        var finder = new UrlFinder();

        var result =await  finder.FindUrls(content,"http://1337.tech").ToListAsync();
        result.Should()
            .NotBeEmpty()
            .And.AllSatisfy(c=>c.Should().StartWith("http"))
            .And.HaveCount(10);
    }


}
