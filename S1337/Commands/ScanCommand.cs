using System.IO.Abstractions;
using S1337.Core;
using Spectre.Console;

namespace S1337.Commands;

public class ScanCommand:ConsoleAppBase
{
    private readonly IScanner _scanner;
    private readonly IFileSystem _fileSystem;
    readonly List<ScanResult> _downloadLinks = new();
    public ScanCommand(IScanner scanner, IFileSystem fileSystem)
    {
        _scanner = scanner;
        _fileSystem = fileSystem;
    }

    [Command("Scan")]
    public async Task Execute(string url,string filePath="")
    {
        await AnsiConsole.Status().StartAsync("[yellow]Scanning[/]", async ctx =>
        {
            await foreach (var item in _scanner.Scan(url))
            {
                _downloadLinks.Add(item);
                WriteScanResult(item);
                ctx.Status($"[yellow] ({_downloadLinks.Count}) Scanning[/]");
            }
        });
        AnsiConsole.MarkupLine($"[yellow] ({_downloadLinks.Count}) Scanned[/]");
        await SaveToFile(filePath);
    }

    async Task SaveToFile(string path)
    {
        if(string.IsNullOrWhiteSpace(path)) return;
        AnsiConsole.MarkupLine("saving...");
        if (!path.IsDirectory())
        {
            AnsiConsole.MarkupLine("[red]Wrong Path[/]");
            return;
        }

        var lines = _downloadLinks.Select(r => $"{r.Url} - {LinkState(r.State)}");
        var filePath = _fileSystem.Path.Combine(path, "Links.txt");
        await _fileSystem.File.WriteAllLinesAsync(filePath,lines);
        AnsiConsole.MarkupLine($"report is ready : [green]{filePath}[/]");
    }

    string LinkState(ScanState state) => state == ScanState.Success ? "Success" : "Fail";

    void WriteScanResult(ScanResult result)
    {
        var color = result.State == ScanState.Success ? "green" : "red";
        AnsiConsole.MarkupLine($"[{color}]*:[/] {result.Url}[{color}]...[/]");
    }
}