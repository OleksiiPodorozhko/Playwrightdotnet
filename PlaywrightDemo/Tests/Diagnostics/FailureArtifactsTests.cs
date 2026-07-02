using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using PlaywrightDemo.Base;

namespace PlaywrightDemo.Tests.Diagnostics;

[TestFixture]
[Category("Diagnostics")]
[AllureFeature("Diagnostics")]
public class FailureArtifactsTests : TodoMvcTestBase
{
    [Test]
    [AllureName("Demo failure produces Playwright artifacts")]
    [AllureStory("Failure artifacts")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task DemoFailureAttachesTraceScreenshotAndVideo()
    {
        if (Environment.GetEnvironmentVariable("RUN_FAILURE_DEMO") != "true")
        {
            Assert.Ignore("Set RUN_FAILURE_DEMO=true to intentionally run the failure artifact demo.");
        }

        await TestStep("Open TodoMVC app", OpenTodoMvcAsync);

        await TestStep("Fail intentionally after page interaction", async () =>
        {
            await TodoMvcPage.AddTodoAsync("This todo exists only for failure artifact verification");
            Assert.That(await TodoMvcPage.TodoLabel("This todo exists only for failure artifact verification").TextContentAsync(),
                Is.EqualTo("A deliberately wrong title"));
        });
    }
}
