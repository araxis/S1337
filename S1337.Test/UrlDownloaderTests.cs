using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using S1337.Core;
using S1337.Services;
using Xunit;

namespace S1337.Test;

public class UrlDownloaderTests
{

    [Fact]
    public async Task DownloadMethodMustDownloadFileAndSaveItOnDisk()
    {
        var url = "http://1337.tech/images/a.jpg";
        var destination = @"D:\site";
        var onDiskPath = @"D:\site\images\a.jpg";
        var fileSystem = new Mock<IFileSystem>();
        fileSystem.Setup(f => f.File.Open(onDiskPath, FileMode.Create))
            .Returns(new MemoryStream())
            .Verifiable();
        var handler = new Mock<HttpMessageHandler>();
        var client = handler.CreateClient();
        handler.SetupRequest(HttpMethod.Get, url).ReturnsResponse(url, "text/html");
        var filePathResolver = new Mock<IFilePathResolver>();
        filePathResolver.Setup(fp => fp.ResolvePath(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(onDiskPath);

        var downloader = new UrlDownloader(client, filePathResolver.Object, fileSystem.Object);
        var resultStream = downloader.Download(url, destination, default);
        var reports = new List<DownloadState>();
        await foreach (var report in resultStream)
        {
            reports.Add(report);
        }
        fileSystem.Verify(f => f.File.Open(onDiskPath,FileMode.Create), Times.Once);
        reports.Should().NotBeEmpty();
    }
}