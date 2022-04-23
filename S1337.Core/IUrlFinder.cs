namespace S1337.Core;

public interface IUrlFinder
{
    IEnumerable<string> FindUrls(string content);
}