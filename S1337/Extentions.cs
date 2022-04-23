using System.Runtime.CompilerServices;

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
}