namespace S1337.Core
{
    public interface IUrlDownloader
    {
        IAsyncEnumerable<DownloadState> Download(string url, string destination, CancellationToken cancellationToken);
    }
}
