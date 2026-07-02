using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using PlaywrightDemo.Artifacts;

namespace PlaywrightDemo.Base;

[AllureNUnit]
[Parallelizable(ParallelScope.Self)]
public abstract class TestBase : PageTest
{
    private TestArtifactPaths? _artifactPaths;
    private string? _videoRecordingDirectory;

    protected TestArtifactPaths ArtifactPaths => _artifactPaths ??= TestArtifactPaths.GetForCurrentTest();

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            RecordVideoDir = VideoRecordingDirectory,
            RecordVideoSize = new RecordVideoSize
            {
                Width = 1280,
                Height = 720
            },
            ViewportSize = new ViewportSize
            {
                Width = 1280,
                Height = 720
            }
        };
    }

    private string VideoRecordingDirectory
    {
        get
        {
            if (_videoRecordingDirectory is not null)
            {
                return _videoRecordingDirectory;
            }

            var tempRoot = Path.Combine(Path.GetTempPath(), "PlaywrightDemo", "videos");
            var safeTestName = ArtifactNameHelper.SafeTestName(TestContext.CurrentContext.Test.FullName);
            _videoRecordingDirectory = Path.Combine(tempRoot, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{safeTestName}");
            Directory.CreateDirectory(_videoRecordingDirectory);

            return _videoRecordingDirectory;
        }
    }

    protected Task TestStep(string name, Func<Task> action)
    {
        return AllureApi.Step(name, action);
    }

    protected Task<T> TestStep<T>(string name, Func<Task<T>> action)
    {
        return AllureApi.Step(name, action);
    }

    protected void TestStep(string name, Action action)
    {
        AllureApi.Step(name, action);
    }

    protected T TestStep<T>(string name, Func<T> action)
    {
        return AllureApi.Step(name, action);
    }

    [SetUp]
    public async Task StartTraceAsync()
    {
        _artifactPaths = TestArtifactPaths.GetForCurrentTest();

        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Title = TestContext.CurrentContext.Test.FullName,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task CaptureFailureArtifactsAsync()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var video = Page.Video;

        if (failed)
        {
            await CapturePageFailureArtifactsAsync();
        }

        await StopTracingAsync(failed);
        await FinalizeVideoAsync(video, failed);

        if (!failed)
        {
            ArtifactPaths.DeleteTestDirectoryIfExists();
        }
    }

    private async Task CapturePageFailureArtifactsAsync()
    {
        ArtifactPaths.EnsureDirectories();

        await File.WriteAllTextAsync(ArtifactPaths.UrlPath, Page.Url);
        await File.WriteAllTextAsync(ArtifactPaths.HtmlPath, await Page.ContentAsync());
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            FullPage = true,
            Path = ArtifactPaths.ScreenshotPath
        });

        AllureAttachmentHelper.AttachTextFile("Current page URL", ArtifactPaths.UrlPath);
        AllureAttachmentHelper.AttachHtmlFile("Page HTML", ArtifactPaths.HtmlPath);
        AllureAttachmentHelper.AttachPngFile("Screenshot", ArtifactPaths.ScreenshotPath);
    }

    private async Task StopTracingAsync(bool failed)
    {
        try
        {
            await Context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = failed ? ArtifactPaths.TracePath : null
            });

            if (failed)
            {
                AllureAttachmentHelper.AttachZipFile("Playwright trace", ArtifactPaths.TracePath);
            }
        }
        catch (Exception exception)
        {
            TestContext.Progress.WriteLine($"Unable to stop Playwright tracing: {exception}");
        }
    }

    private async Task FinalizeVideoAsync(IVideo? video, bool failed)
    {
        try
        {
            await Page.CloseAsync();

            if (video is null)
            {
                return;
            }

            var videoPath = await video.PathAsync();

            if (failed)
            {
                ArtifactPaths.EnsureDirectories();

                var retainedVideoPath = ArtifactPaths.VideoPath(videoPath);
                File.Copy(videoPath, retainedVideoPath, overwrite: true);

                AllureAttachmentHelper.AttachWebmFile("Playwright video", retainedVideoPath);
                return;
            }

            await video.DeleteAsync();
        }
        catch (Exception exception)
        {
            TestContext.Progress.WriteLine($"Unable to finalize Playwright video: {exception}");
        }
        finally
        {
            DeleteVideoRecordingDirectoryIfExists();
        }
    }

    private void DeleteVideoRecordingDirectoryIfExists()
    {
        if (_videoRecordingDirectory is null || !Directory.Exists(_videoRecordingDirectory))
        {
            return;
        }

        try
        {
            Directory.Delete(_videoRecordingDirectory, recursive: true);
        }
        catch (Exception exception)
        {
            TestContext.Progress.WriteLine($"Unable to delete temporary video directory: {exception}");
        }
    }
}
