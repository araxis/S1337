namespace S1337.Core;

public interface IRequestUriBuilder
{
    Uri Build(string rawUrl, string domain);
}