using S1337.Core;
using Spectre.Console;

namespace S1337.Commands;

public class AppRootCommand : ConsoleAppBase
{
    private readonly IScanner _scanner;
    private readonly IUrlDownloader _downloader;

    public AppRootCommand(IScanner scanner, IUrlDownloader downloader)
    {

        _scanner = scanner;
        _downloader = downloader;

    }

    [RootCommand]
    public async Task Execute()
    {
        AnsiConsole.Write(new FigletText("1337.tech").Centered().Color(Color.Green));
        AnsiConsole.Write(new FigletText("Araxis").Centered().Color(Color.Red));
        AnsiConsole.MarkupLine("[bold yellow]please enter full[/] [red]url![/]");
        AnsiConsole.MarkupLine("if you are 1337, can leave it blank :)");
        var url = AnsiConsole.Ask<string>("site url[green][/] :", "http://1337.tech");
        if (string.IsNullOrWhiteSpace(url)) url = "http://1337.tech";
        while (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            url = AnsiConsole.Ask<string>("site [green]url[/] :");
        }
        
        var downloadLinks = new List<ScanResult>();

        await AnsiConsole.Status().StartAsync("[yellow]Scanning[/]", async ctx =>
        {
            await foreach (var item in _scanner.Scan(url))
            {
                downloadLinks.Add(item);
                WriteScanResult(item);
                ctx.Status($"[yellow] ({downloadLinks.Count}) Scanning[/]");
            }
        });


        var download = AnsiConsole.Confirm("Download Site?");
        if (download)
        {
            var path = AnsiConsole.Ask<string>("enter save folder [green]path[/].");
            while (!path.IsDirectory())
            {
                path = AnsiConsole.Ask<string>("enter save folder [green]path[/].");
            }

            await AnsiConsole.Progress().AutoClear(false)
                .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new SpinnerColumn())
                .StartAsync(async ctx =>
                {
                    var validLinks = downloadLinks.Where(c => c.State == ScanState.Success);

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

    }
    void WriteScanResult(ScanResult result)
    {
        var color = result.State == ScanState.Success ? "green" : "red";
        AnsiConsole.MarkupLine($"[{color}]*:[/] {result.Url}[{color}]...[/]");
    }
}