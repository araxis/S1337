using System.Text.RegularExpressions;
using S1337.Core;

namespace S1337.Services;

public class UrlFinder:IUrlFinder
{
    public IEnumerable<string> FindUrls(string content)
    {
    
        var hrefPattern = @"href=(?<QUOTE>[\""\'])?(?<URL>(?<SCHEME>(file|ftp|http|https|news|nntp):\/\/|mailto\:)?(?<EMAIL>[\w-]+@)?(?<HOST>(?(SCHEME)[\w]+(\.[\w-]+)*?))(?<PATH>\/?\w*[\w-%\:\.\+\/]+)?(?<QUERY>\?[\w-%\+:\.]*(=[\w-%\+:\.]*)?(&[\w-%\+\:\.]*(=[\w-%\+:\.]*)?)*)?(?<ANCHOR>\#[\w-%\+:\.]+)?)(?<-QUOTE>[\""\'])?(?#VALIDATE QUOTES/URL)(?(PATH)|(?(SCHEME)|(?!)))(?(QUOTE)(?!))";
        foreach (Match match in Regex.Matches(content, hrefPattern, RegexOptions.IgnoreCase))
        {
            yield return match.Groups["URL"].Value;
        }
      
    }
}