using S1337.Core;

namespace S1337.Services;

public class RequestUriBuilder : IRequestUriBuilder
{
    public Uri Build(string rawUrl, string domain)
    {
        var url = rawUrl.Trim('/');
        if (url.StartsWith("www.")) url = $"http://{url}";
        var isRelative = Uri.IsWellFormedUriString(url, UriKind.Relative);
        if (!isRelative) return new Uri(url);
        var builder = new UriBuilder(domain)
        {
            Path = url
        };
        return builder.Uri;
    }
}