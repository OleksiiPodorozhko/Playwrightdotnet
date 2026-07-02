using Microsoft.Playwright;

namespace PlaywrightDemo.Pages;

public class TodoMvcPage
{
    private readonly IPage _page;

    public TodoMvcPage(IPage page)
    {
        _page = page;
    }

    public ILocator Heading => _page.GetByRole(AriaRole.Heading, new() { Name = "todos" });

    public ILocator NewTodoInput => _page.GetByPlaceholder("What needs to be done?");

    public ILocator Main => _page.Locator(".main");

    public ILocator Footer => _page.Locator(".footer");

    public ILocator TodoItems => _page.Locator(".todo-list li");

    public ILocator ActiveTodoItems => _page.Locator(".todo-list li:not(.completed)");

    public ILocator CompletedTodoItems => _page.Locator(".todo-list li.completed");

    public ILocator TodoCounter => _page.Locator(".todo-count");

    public ILocator AllFilter => _page.GetByRole(AriaRole.Link, new() { Name = "All" });

    public ILocator ActiveFilter => _page.GetByRole(AriaRole.Link, new() { Name = "Active" });

    public ILocator CompletedFilter => _page.GetByRole(AriaRole.Link, new() { Name = "Completed" });

    public ILocator ClearCompletedButton => _page.Locator(".clear-completed");

    public ILocator TodoItem(string title)
    {
        return TodoItems.Filter(new() { HasText = title });
    }

    public ILocator TodoLabel(string title)
    {
        return TodoItem(title).Locator("label");
    }

    public ILocator TodoToggle(string title)
    {
        return TodoItem(title).Locator(".toggle");
    }

    public async Task AddTodoAsync(string title)
    {
        await NewTodoInput.FillAsync(title);
        await NewTodoInput.PressAsync("Enter");
    }

    public async Task AddTodosAsync(params string[] titles)
    {
        foreach (var title in titles)
        {
            await AddTodoAsync(title);
        }
    }

    public Task CompleteTodoAsync(string title)
    {
        return TodoToggle(title).CheckAsync();
    }

    public Task UncompleteTodoAsync(string title)
    {
        return TodoToggle(title).UncheckAsync();
    }

    public async Task DeleteTodoAsync(string title)
    {
        var todoItem = TodoItem(title);

        await todoItem.HoverAsync();
        await todoItem.Locator(".destroy").ClickAsync();
    }

    public Task ClearCompletedAsync()
    {
        return ClearCompletedButton.ClickAsync();
    }

    public async Task VerifyVisibleTodosAsync(params string[] titles)
    {
        await Assertions.Expect(TodoItems).ToHaveCountAsync(titles.Length);

        for (var index = 0; index < titles.Length; index++)
        {
            await Assertions.Expect(TodoItems.Nth(index).Locator("label")).ToHaveTextAsync(titles[index]);
        }
    }

    public async Task VerifyTodoIsActiveAsync(string title)
    {
        await Assertions.Expect(TodoItem(title)).Not.ToHaveClassAsync(new Regex("completed"));
        await Assertions.Expect(TodoToggle(title)).Not.ToBeCheckedAsync();
    }

    public async Task VerifyTodoIsCompletedAsync(string title)
    {
        await Assertions.Expect(TodoItem(title)).ToHaveClassAsync(new Regex("completed"));
        await Assertions.Expect(TodoToggle(title)).ToBeCheckedAsync();
    }
}
