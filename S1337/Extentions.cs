using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using S1337.Core;
using S1337.Services;

namespace S1337;
public record SaveState(double Copied, double Total);
public static class Extensions
{
   
    public static async IAsyncEnumerable<SaveState> CopyToAsync(this Stream source, long sourceLength, Stream destination, int bufferSize = 81920,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {

        var buffer = new byte[bufferSize];
        if (sourceLength < 0 && source.CanSeek)
            sourceLength = source.Length - source.Position;
        var totalBytesCopied = 0L;
        yield return new SaveState(totalBytesCopied, sourceLength);
        int bytesRead ;


        while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
        {
            await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            totalBytesCopied += bytesRead;
            yield return new SaveState(totalBytesCopied, sourceLength);

            if (cancellationToken.IsCancellationRequested) { break; }
        }

   

    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IUrlFinder, UrlFinder>();
        services.AddTransient<IScanner, Scanner>();
        services.AddSingleton<IFilePathResolver, FilePathResolver>();
        services.AddSingleton<IUrlDownloader, UrlDownloader>();
        return services;
    }

    public static IScanner Scanner(this IServiceProvider provider) => provider.GetRequiredService<IScanner>();
    public static IUrlDownloader UrlDownloader(this IServiceProvider provider) => provider.GetRequiredService<IUrlDownloader>();

    public static bool IsDirectory(this string path)
    {
        var fileExt = Path.GetExtension(path);
        return string.IsNullOrWhiteSpace(fileExt);
  
       
    }

    //to SCHEME + DOMAIN 😎😎😎😎😎
    public static string ToSD(this Uri uri)
    {
        return $"{uri.Scheme}://{uri.Authority}";


    }

}
