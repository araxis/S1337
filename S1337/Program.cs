using Microsoft.Extensions.Logging;
using S1337;
using S1337.Commands;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) => {
    services.AddAppServices();

}).ConfigureLogging(ctx =>
{
    ctx.ClearProviders();
    ctx.AddSerilog();
}).UseSerilog();


var app = builder.Build();

app.AddCommands<AppRootCommand>();
app.AddCommands<DownloadCommand>();
app.AddCommands<ScanCommand>();
Console.CancelKeyPress += (_, e) =>
{
    Environment.Exit(0);
};
await app.RunAsync();
