namespace S1337.Core;

public record DownloadState(string Url, double Copied, double Total)
{
    public double Percentage
    {
        get
        {
            if (Total == 0) return 0;
            return (100 * Copied) / Total;
        }
    }
}