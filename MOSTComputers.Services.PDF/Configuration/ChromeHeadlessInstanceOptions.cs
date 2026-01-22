namespace MOSTComputers.Services.PDF.Configuration;

public sealed class ChromeHeadlessInstanceOptions
    {
        public required int DebuggingPortNumber { get; init; }
        public required string LocalUserDataDirectoryPath { get; init; }
    }
