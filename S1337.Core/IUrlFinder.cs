namespace S1337.Core;

public interface IUrlFinder
{
    IAsyncEnumerable<string> FindUrls(string content,string baseUrl);
   
}