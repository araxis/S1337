
using Microsoft.Extensions.DependencyInjection;
using S1337;
using S1337.Core;
using Spectre.Console;


var serviceProvider = new ServiceCollection().AddAppServices()
                                             .BuildServiceProvider();



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
var scanner = serviceProvider.Scanner();
await AnsiConsole.Status().StartAsync("[yellow]Scanning[/]", async ctx =>
{
    await foreach (var item in scanner.Scan(url))
    {
        downloadLinks.Add(item);
        WriteScanResult(item);
        ctx.Status($"[yellow] ({downloadLinks.Count}) Scanning[/]");
    }
    //ctx.Status($"[yellow] ({downloadLinks.Count}) Scanned[/]");
});


var download = AnsiConsole.Confirm("Download Site?");
if (download)
{
    var path = AnsiConsole.Ask<string>("enter save folder [green]path[/].");
    while (!path.IsDirectory())
    {
        path = AnsiConsole.Ask<string>("enter save folder [green]path[/].");
    }

    var downloader = serviceProvider.UrlDownloader();
    await AnsiConsole.Progress().AutoClear(false)
        .Columns(new ProgressColumn[]
        {

            new TaskDescriptionColumn(),    // Task description
            new ProgressBarColumn(),        // Progress bar
            new PercentageColumn(),         // Percentage
            new SpinnerColumn(),            // Spinner
        }).StartAsync(async ctx =>
        {
            var validLinks = downloadLinks.Where(c => c.State == ScanState.Success);

            foreach (var link in validLinks)
            {
                var task = ctx.AddTask($"[green]{link.Url}[/]");
                await foreach (var state in downloader.Download(link.Url, path, default))
                {
                    task.Increment(state.Percentage);
                }
            }

        });
}




void WriteScanResult(ScanResult result)
{
    var color = result.State == ScanState.Success ? "green" : "red";
    AnsiConsole.MarkupLine($"[{color}]*:[/] {result.Url}[{color}]...[/]");
}
