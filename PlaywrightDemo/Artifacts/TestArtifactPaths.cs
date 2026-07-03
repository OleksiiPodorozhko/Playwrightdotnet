namespace PlaywrightDemo.Artifacts;

public sealed class TestArtifactPaths
{
    private const string TimestampFormat = "yyyy-MM-dd-HH-mm";

    private TestArtifactPaths(string testDirectory)
    {
        TestDirectory = testDirectory;
        ScreenshotPath = Path.Combine(testDirectory, "failure.png");
        TracePath = Path.Combine(testDirectory, "trace.zip");
        VideoPath = Path.Combine(testDirectory, "video.webm");
        HtmlPath = Path.Combine(testDirectory, "page.html");
        UrlPath = Path.Combine(testDirectory, "url.txt");
    }

    public string TestDirectory { get; }

    public string ScreenshotPath { get; }

    public string TracePath { get; }

    public string VideoPath { get; }

    public string HtmlPath { get; }

    public string UrlPath { get; }

    public void EnsureDirectories()
    {
        Directory.CreateDirectory(TestDirectory);
        Directory.CreateDirectory(Path.Combine(ArtifactNameHelper.RepositoryRoot, "test-results", "allure-results"));
        Directory.CreateDirectory(Path.Combine(ArtifactNameHelper.RepositoryRoot, "test-results", "allure-report"));
    }

    public void DeleteTestDirectoryIfExists()
    {
        if (Directory.Exists(TestDirectory))
        {
            Directory.Delete(TestDirectory, recursive: true);
        }
    }

    public static TestArtifactPaths GetForCurrentTest()
    {
        var timestamp = DateTime.Now.ToString(TimestampFormat);
        return CreateForTest(TestContext.CurrentContext.Test.FullName, timestamp);
    }

    private static TestArtifactPaths CreateForTest(string testName, string timestamp)
    {
        var safeTestName = ArtifactNameHelper.SafeTestName(testName);
        var folderName = $"{timestamp}_{safeTestName}";
        return new TestArtifactPaths(Path.Combine(ArtifactNameHelper.RepositoryRoot, "test-results", "artifacts", folderName));
    }
}
