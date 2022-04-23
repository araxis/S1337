using S1337.Core;

namespace S1337.Services;

public class Scanner:IScanner
{
    private readonly IUrlFinder _urlFinder;
    private readonly HttpClient _client;
    private readonly IRequestUriBuilder _requestUriBuilder;
    private readonly List<string> _alreadyScanned=new();
    private string _domain="";
    public Scanner(IUrlFinder urlFinder, HttpClient client, IRequestUriBuilder requestUriBuilder)
    {
        _urlFinder = urlFinder;
        _client = client;
        _requestUriBuilder = requestUriBuilder;
    }



    public async IAsyncEnumerable<ScanResult> Scan(string url)
    {
        //fetch domain from first request
        if (string.IsNullOrWhiteSpace(this._domain)) this._domain = new Uri(url).Host;
        var uri = _requestUriBuilder.Build(url, _domain);
        //reject other domain url
        //It is better if this logic moves out of this class
        if (uri.Host != _domain) yield break;

        var request = uri.AbsoluteUri;
        if (_alreadyScanned.Contains(request)) yield break;
        _alreadyScanned .Add(request);

       
      
    
  
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

}