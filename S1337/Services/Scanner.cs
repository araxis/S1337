using S1337.Core;

namespace S1337.Services;

public class Scanner:IScanner
{
    private readonly IUrlFinder _urlFinder;
    private readonly HttpClient _client;

    public Scanner(IUrlFinder urlFinder, HttpClient client)
    {
        _urlFinder = urlFinder;
        _client = client;
    }

    public async IAsyncEnumerable<ScanResult> Scan(string url)
    {
        var request = MakeRequestUrl(url);
        if (string.IsNullOrWhiteSpace(request)) yield break;
        var content = "";
       //There should be a better way to handle this
        var error = false;
        try
        {
            using var response = await _client.GetAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (MustScan(response))
            {
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            //bad block of code
            //the exception must be logged
            error=true;
          
        }

        if (error)
        {
            yield return new ScanResult(url,ScanState.Fail);
        }
        else
        {
            yield return new ScanResult(url, ScanState.Success);
            var scannedUrls = _urlFinder.FindUrls(content);
            foreach (var scannedUrl in scannedUrls.Distinct())
            {
                await foreach (var s in Scan(scannedUrl))
                {
                    yield return s;
                }
            }
        }
      
     
    }

    private string MakeRequestUrl(string url)
    {
        Uri uri = null;
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            uri = new Uri(url);
        }
        if (Uri.IsWellFormedUriString(url, UriKind.Relative))
        {
            uri = new UriBuilder("http://1337.tech")
            {
                Path = url
            }.Uri;
        }
        if (uri == null) return string.Empty;
        if (uri.Scheme != "http" && uri.Scheme != "https") return string.Empty;
        var ret = uri.AbsoluteUri;
        return ret;
    }


    private bool MustScan(HttpResponseMessage response)
    {
        return response.Content.Headers.ContentType?.MediaType == "text/html";
    }
}