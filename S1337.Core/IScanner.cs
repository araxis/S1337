namespace S1337.Core;

public interface IScanner
{
    IAsyncEnumerable<ScanResult> Scan(string url);
}

