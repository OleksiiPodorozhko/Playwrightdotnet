using PlaywrightDemo.Pages;

namespace PlaywrightDemo.Base;

public abstract class TodoMvcTestBase : TestBase
{
    protected const string TodoMvcUrl = "https://demo.playwright.dev/todomvc/#/";

    protected TodoMvcPage TodoMvcPage { get; private set; } = null!;

    [SetUp]
    public void SetUpTodoMvcTest()
    {
        TodoMvcPage = new TodoMvcPage(Page);
    }

    protected Task OpenTodoMvcAsync()
    {
        return Page.GotoAsync(TodoMvcUrl);
    }
}
