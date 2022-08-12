[![build app](https://github.com/araxis/S1337/actions/workflows/dotnet.yml/badge.svg)](https://github.com/araxis/S1337/actions/workflows/dotnet.yml)
### S1337

💡S1337 is a console application for scraping websites

# Notice it is just a demo 😎😎
Get the repo, extract it.
Go to S1337 folder and run the following command.

``` csharp
dotnet run
```

<p align="center">
  <img width="80%" height="100%" src="https://user-images.githubusercontent.com/1418779/164895285-b49600a1-1689-4e5a-8aac-3fb88feb4f44.png">
</p>
### Third-party libraries that are used.

- [MediaTypeMap.Core](https://github.com/samuelneff/MimeTypeMap) Mapping response mime type to file extension
- [System.IO.Abstractions](https://github.com/TestableIO/System.IO.Abstractions)  injectable and testable IO related operations
- [Spectre.Console](https://github.com/spectreconsole/spectre.console) for design gui

### Third-party libraies that are used in the test project

- [FluentAssertions](https://fluentassertions.com/) 
- [moq](https://github.com/moq/moq4) 
- [Moq.Contrib.HttpClient](https://github.com/maxkagamine/Moq.Contrib.HttpClient) for mocking HttpClient 


## Roadmap

- [x] Improve Scanning
- [x] Improve Error handling
- [x] Add logging
- [x] Cli
- [ ] Batch Download

