using Microsoft.Extensions.Logging;
using S1337.Core;

namespace S1337.Services;

public class Scanner:IScanner
{
    private readonly IUrlFinder _urlFinder;
    private readonly HttpClient _client;
    private readonly ILogger<Scanner> _logger;
    private readonly List<string> _alreadyScanned=new();
    private Uri? _domain;
    public Scanner(IUrlFinder urlFinder, HttpClient client, ILogger<Scanner> logger)
    {
        _urlFinder = urlFinder;
        _client = client;
        _logger = logger;
    }



    public async IAsyncEnumerable<ScanResult> Scan(string url)
    {
        var uri = new Uri(url);
        //fetch domain from first request
        _domain ??= uri;

      
        //reject other domain url
        //It is better if this logic moves out of this class
        if (uri.Host != _domain.Host) yield break;

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
            _logger.LogError("Downloading error:{Elapsed}",e.Message);
          
        }

        if (error)
        {
            yield return new ScanResult(request, ScanState.Fail);
        }
        else
        {
            yield return new ScanResult(request, ScanState.Success);
            await foreach (var scannedUrl in _urlFinder.FindUrls(content,_domain.ToSD()))
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