using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;

namespace PlaywrightDemo.Base;

[AllureNUnit]
[Parallelizable(ParallelScope.Self)]
public abstract class TestBase : PageTest
{
    protected const string TodoMvcUrl = "https://demo.playwright.dev/todomvc/#/";

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

    protected async Task OpenCleanAppAsync(string url)
    {
        await Page.GotoAsync(url);
        await Page.EvaluateAsync("() => localStorage.clear()");
        await Page.ReloadAsync();
    }

    [TearDown]
    public async Task AttachFailureArtifactsAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
        {
            return;
        }

        AllureApi.AddAttachment(
            "Current page URL",
            "text/plain",
            System.Text.Encoding.UTF8.GetBytes(Page.Url),
            ".txt");

        AllureApi.AddAttachment(
            "Page HTML",
            "text/html",
            System.Text.Encoding.UTF8.GetBytes(await Page.ContentAsync()),
            ".html");

        AllureApi.AddAttachment(
            "Screenshot",
            "image/png",
            await Page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true }),
            ".png");
    }
}
