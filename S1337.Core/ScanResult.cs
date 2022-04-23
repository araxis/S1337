namespace S1337.Core;

public enum ScanState{Success,Fail}
public record ScanResult(string Url,ScanState State );
