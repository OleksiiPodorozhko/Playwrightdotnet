using PlaywrightDemo.Models;
using PlaywrightDemo.Pages;
using PlaywrightDemo.TestData;

namespace PlaywrightDemo.Base;

public abstract class TodoMvcTestBase : TestBase
{
    protected const string TodoMvcUrl = "https://demo.playwright.dev/todomvc/#/";

    protected TodoMvcPage TodoMvc { get; private set; } = null!;

    [SetUp]
    public void SetUpTodoMvcTest()
    {
        TodoMvc = new TodoMvcPage(Page);
    }

    protected async Task OpenTodoMvcWithCleanStorageAsync()
    {
        await Page.GotoAsync(TodoMvcUrl);
        await Page.EvaluateAsync("() => localStorage.clear()");
        await Page.ReloadAsync();
    }

    protected async Task VerifyVisibleTodosAsync(params string[] titles)
    {
        await Expect(TodoMvc.TodoItems).ToHaveCountAsync(titles.Length);

        for (var index = 0; index < titles.Length; index++)
        {
            await Expect(TodoMvc.TodoItems.Nth(index).Locator("label")).ToHaveTextAsync(titles[index]);
        }
    }

    protected async Task VerifyTodoIsActiveAsync(string title)
    {
        await Expect(TodoMvc.TodoItem(title)).Not.ToHaveClassAsync(new Regex("completed"));
        await Expect(TodoMvc.TodoToggle(title)).Not.ToBeCheckedAsync();
    }

    protected async Task VerifyTodoIsCompletedAsync(string title)
    {
        await Expect(TodoMvc.TodoItem(title)).ToHaveClassAsync(new Regex("completed"));
        await Expect(TodoMvc.TodoToggle(title)).ToBeCheckedAsync();
    }

    protected async Task VerifyStoredTodosAsync(params ExpectedTodo[] expectedTodos)
    {
        var storedTodos = await TodoMvc.GetStoredTodosAsync();

        Assert.That(storedTodos, Has.Count.EqualTo(expectedTodos.Length));

        for (var index = 0; index < expectedTodos.Length; index++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(storedTodos[index].Title, Is.EqualTo(expectedTodos[index].Title));
                Assert.That(storedTodos[index].Completed, Is.EqualTo(expectedTodos[index].Completed));
            });
        }
    }

    protected static ExpectedTodo ExpectedActive(string title)
    {
        return new ExpectedTodo(title, false);
    }

    protected static ExpectedTodo ExpectedCompleted(string title)
    {
        return new ExpectedTodo(title, true);
    }
}
