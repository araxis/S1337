using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using S1337.Core;

namespace S1337.Services;

public class UrlFinder : IUrlFinder
{


    public async IAsyncEnumerable<string> FindUrls(string content, string baseUrl)
    {
        var cfg =Configuration.Default;
    
        var document = await BrowsingContext.New(cfg).OpenAsync(
            res => res.Content(content).Address(baseUrl));

        var elements = document.All
            .Where(m => m.LocalName is "img" or "script" or "a" or "link").ToList();

        foreach (var element in elements)
        {
            var link = element.LocalName switch
            {
                "script" => element.HyperReference((element as IHtmlScriptElement)?.Source ?? string.Empty).Href,
                "a" => ((IHtmlAnchorElement) element).Href,
                "link" => ((IHtmlLinkElement) element).Href,
                _ => ""
            };
            if (!string.IsNullOrWhiteSpace(link)) yield return link;
        }

        var images = elements.OfType<IHtmlImageElement>().ToList();
        foreach (var img in images)
        {
            if (!string.IsNullOrWhiteSpace(img.Source)) yield return img.Source;
            var srcSet = img.SourceSet;
            if (string.IsNullOrWhiteSpace(srcSet)) continue;
            var urls = srcSet.Split(',');
            foreach (var link in urls)
            {
                yield return img.HyperReference(link).Href;
            }

        }
    }


   
}