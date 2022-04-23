using System.Text.RegularExpressions;
using S1337.Core;

namespace S1337.Services;

public class UrlFinder:IUrlFinder
{
    public IEnumerable<string> FindUrls(string content)
    {
        const string hrefPattern = @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>[^>\s]+))";
        var regexMatch = Regex.Match(content, hrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        while (regexMatch.Success)
        {

            var matched = regexMatch.Groups[1].Value;
            yield return matched.TrimEnd('/');
            regexMatch = regexMatch.NextMatch();

        }
    }
}