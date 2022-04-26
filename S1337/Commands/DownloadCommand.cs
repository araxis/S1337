using Microsoft.Extensions.Logging;
using S1337.Core;
using Spectre.Console;

namespace S1337.Commands;

public class DownloadCommand:ConsoleAppBase
{
    private readonly IScanner _scanner;
    private readonly IUrlDownloader _downloader;
    private readonly ILogger<DownloadCommand> _logger;
    readonly List<ScanResult> _downloadLinks = new();
    public DownloadCommand(IScanner scanner, IUrlDownloader downloader, ILogger<DownloadCommand> logger)
    {
        _scanner = scanner;
        _downloader = downloader;
        _logger = logger;
    }



    [Command("Download")]
    public async Task Execute([Option("f", "folder path")] string path,
        [Option("u", "address of site")] string url="http://1337.tech")
    {
        if (!path.IsDirectory())
        {
            _logger.LogError("Wrong Path{Path}",path);
            return;
        }
        _logger.LogInformation("start executing");
        AnsiConsole.Write(new FigletText("1337.tech").Centered().Color(Color.Green));
        AnsiConsole.Write(new FigletText("Araxis").Centered().Color(Color.Red));
        await Scan(url);


        await Download(path);
    }

    private async Task Download(string path)
    {


        await AnsiConsole.Progress().AutoClear(false)
                .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new SpinnerColumn())
                .StartAsync(async ctx =>
                {
                    var validLinks = _downloadLinks.Where(c => c.State == ScanState.Success);

                    foreach (var link in validLinks)
                    {
                        var task = ctx.AddTask($"[green]{link.Url}[/]");
                        await foreach (var state in _downloader.Download(link.Url, path, default))
                        {
                            task.Increment(state.Percentage);
                        }
                    }
                });
      
    }

    private Task Scan(string url)
    {
        return AnsiConsole.Status().StartAsync("[yellow]Scanning[/]", async ctx =>
        {
            await foreach (var item in _scanner.Scan(url))
            {
                _downloadLinks.Add(item);
                WriteScanResult(item);
                ctx.Status($"[yellow] ({_downloadLinks.Count}) Scanning[/]");
            }
            //ctx.Status($"[yellow] ({downloadLinks.Count}) Scanned[/]");
        });
    }

    void WriteScanResult(ScanResult result)
    {
        var color = result.State == ScanState.Success ? "green" : "red";
        AnsiConsole.MarkupLine($"[{color}]*:[/] {result.Url}[{color}]...[/]");
    }
}