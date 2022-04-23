using S1337.Core;

namespace S1337.Services;

public class Scanner:IScanner
{
    private readonly IUrlFinder _urlFinder;
    private readonly HttpClient _client;
    private readonly IRequestUriBuilder _requestUriBuilder;
    private readonly List<string> _alreadyScanned=new();

    public Scanner(IUrlFinder urlFinder, HttpClient client, IRequestUriBuilder requestUriBuilder)
    {
        _urlFinder = urlFinder;
        _client = client;
        _requestUriBuilder = requestUriBuilder;
    }



    public async IAsyncEnumerable<ScanResult> Scan(string url)
    {
        if(_alreadyScanned.Contains(url)) yield break;
        _alreadyScanned .Add(url);

        var domain = GetDomain();
        var request = _requestUriBuilder.Build(url, domain).AbsoluteUri;

        var content = "";
       //There should be a better way to handle this
        var error = false;
        try
        {
            using var response = await _client.GetAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                error =true;
            }
            else if (MustScan(response))
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
            yield return new ScanResult(request, ScanState.Fail);
        }
        else
        {
            yield return new ScanResult(request, ScanState.Success);
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




    private bool MustScan(HttpResponseMessage response)
    {
       
        return response.Content.Headers.ContentType?.MediaType == "text/html";
    }

    private string GetDomain()
    {
       var first= _alreadyScanned.First();
       return new Uri(first).Host;
    }
}