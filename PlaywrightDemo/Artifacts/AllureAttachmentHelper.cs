using Allure.Net.Commons;

namespace PlaywrightDemo.Artifacts;

public static class AllureAttachmentHelper
{
    public static void AttachTextFile(string name, string path)
    {
        AttachFile(name, "text/plain", path, ".txt");
    }

    public static void AttachHtmlFile(string name, string path)
    {
        AttachFile(name, "text/html", path, ".html");
    }

    public static void AttachPngFile(string name, string path)
    {
        AttachFile(name, "image/png", path, ".png");
    }

    public static void AttachZipFile(string name, string path)
    {
        AttachFile(name, "application/zip", path, ".zip");
    }

    public static void AttachWebmFile(string name, string path)
    {
        AttachFile(name, "video/webm", path, ".webm");
    }

    private static void AttachFile(string name, string contentType, string path, string extension)
    {
        if (!File.Exists(path))
        {
            TestContext.Progress.WriteLine($"Allure attachment was skipped because file does not exist: {path}");
            return;
        }

        AllureApi.AddAttachment(name, contentType, File.ReadAllBytes(path), extension);
        TestContext.AddTestAttachment(path, name);
    }
}
