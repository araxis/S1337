using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using S1337.Core;

namespace S1337.Services;

public class UrlDownloader:IUrlDownloader
{
 
    private readonly HttpClient _client;
    private readonly IFilePathResolver _filepathResolver;
    private readonly IFileSystem _fileSystem;


    public UrlDownloader(HttpClient client, IFilePathResolver filepathResolver, IFileSystem fileSystem)
    {
        
        _client = client;
        _filepathResolver = filepathResolver;
        _fileSystem = fileSystem;
    }

    public async IAsyncEnumerable<DownloadState> Download(string url, string destination, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        
        using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        var contentLength = response.Content.Headers.ContentLength ?? 0;
        var streamToReadFrom = await response.Content.ReadAsStreamAsync(cancellationToken);
        var savePath = _filepathResolver.ResolvePath(url, destination, response.Content.Headers.ContentType?.MediaType);

        if (savePath.IsDirectory())
        {
            _fileSystem.Directory.CreateDirectory(savePath);
            //it means operation done.Percentage = 100%
            yield return new DownloadState(url, 1,1);
        }
        MakeFileDirectory(savePath);
        await using var streamToWriteTo = _fileSystem.File.Open(savePath, FileMode.Create);

        await foreach (var state in streamToReadFrom.CopyToAsync(contentLength, streamToWriteTo, 81920, cancellationToken))
        {
            yield return new DownloadState(url, state.Copied, state.Total);
        }
    }



    private void MakeFileDirectory(string path)
    {
        var fileDirectoryPath = _fileSystem.Path.GetDirectoryName(path);
        _fileSystem.Directory.CreateDirectory(fileDirectoryPath);

    }
}