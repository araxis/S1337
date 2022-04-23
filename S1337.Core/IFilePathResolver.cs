namespace S1337.Core;

public interface IFilePathResolver
{
    string ResolvePath(string url, string baseFolder, string? mimeType);
}

