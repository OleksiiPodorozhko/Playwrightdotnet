using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using PlaywrightDemo.Base;
using PlaywrightDemo.Pages;

namespace PlaywrightDemo;

[TestFixture]
public class Codex : TestBase
{
    private const string TodoTitle = "Buy milk";

    [Test]
    [AllureName("Add todo")]
    [AllureFeature("TodoMVC")]
    [AllureStory("Add todo")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-001", Title = "Add todo")]
    public async Task AddTodoCreatesAnActiveTodo()
    {
        var todoPage = new TodoMvcPage(Page);

        await TestStep("Open TodoMVC app with clean storage", async () =>
        {
            await OpenCleanAppAsync(TodoMvcUrl);
        });

        await TestStep("Verify empty TodoMVC page is ready for input", async () =>
        {
            await Expect(Page).ToHaveTitleAsync("React • TodoMVC");
            await Expect(todoPage.Heading).ToBeVisibleAsync();
            await Expect(todoPage.NewTodoInput).ToBeVisibleAsync();
            await Expect(todoPage.NewTodoInput).ToBeFocusedAsync();
            await Expect(todoPage.TodoItems).ToHaveCountAsync(0);
        });

        await TestStep($"Add todo '{TodoTitle}'", async () =>
        {
            await todoPage.AddTodoAsync(TodoTitle);
        });

        await TestStep("Verify added todo is active", async () =>
        {
            await Expect(todoPage.TodoItems).ToHaveCountAsync(1);
            await Expect(todoPage.ActiveTodoItems).ToHaveCountAsync(1);
            await Expect(todoPage.CompletedTodoItems).ToHaveCountAsync(0);
            await Expect(todoPage.TodoLabel(TodoTitle)).ToHaveTextAsync(TodoTitle);
            await Expect(todoPage.TodoToggle(TodoTitle)).Not.ToBeCheckedAsync();
        });

        await TestStep("Verify footer and input state after adding todo", async () =>
        {
            await Expect(todoPage.TodoCounter).ToHaveTextAsync("1 item left");
            await Expect(todoPage.AllFilter).ToBeVisibleAsync();
            await Expect(todoPage.ActiveFilter).ToBeVisibleAsync();
            await Expect(todoPage.CompletedFilter).ToBeVisibleAsync();
            await Expect(todoPage.NewTodoInput).ToHaveValueAsync(string.Empty);
        });

        await TestStep("Verify todo is saved to local storage", async () =>
        {
            var storedTodos = await todoPage.StoredTodosAsync();

            Assert.That(storedTodos, Does.Contain("\"title\":\"Buy milk\""));
            Assert.That(storedTodos, Does.Contain("\"completed\":false"));
        });
    }
}
