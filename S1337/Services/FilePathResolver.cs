using S1337.Core;
using static System.IO.Path;
namespace S1337.Services;

public class FilePathResolver : IFilePathResolver
{
    public string ResolvePath(string url, string baseFolder, string? mimeType)
    {
        var baseDirectory = new DirectoryInfo(baseFolder);
        var uri = new Uri(url);
        var path = uri.LocalPath;
        if (path == "/") return GetFullPath(Combine(baseDirectory.FullName, "index.html"));
        path = path.Trim('/');
        if (IsFileUri(uri))
        {
            return GetFullPath(Combine(baseDirectory.FullName, path));
        }

        var extension = MimeTypes.MimeTypeMap.GetExtension(@mimeType, false);
        return GetFullPath(string.IsNullOrWhiteSpace(extension) 
            ? Combine(baseDirectory.FullName, path) 
            : Combine(baseDirectory.FullName, $"{path}{extension}"));
    }

    private bool IsFileUri(Uri uri)
    {
        var fileInfo = new FileInfo(uri.AbsolutePath);
        return !string.IsNullOrWhiteSpace(fileInfo.Extension);
    }


}