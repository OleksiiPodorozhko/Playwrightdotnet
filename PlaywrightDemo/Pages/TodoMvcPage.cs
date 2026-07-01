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

    public ILocator TodoItems => _page.Locator(".todo-list li");

    public ILocator ActiveTodoItems => _page.Locator(".todo-list li:not(.completed)");

    public ILocator CompletedTodoItems => _page.Locator(".todo-list li.completed");

    public ILocator TodoCounter => _page.Locator(".todo-count");

    public ILocator AllFilter => _page.GetByRole(AriaRole.Link, new() { Name = "All" });

    public ILocator ActiveFilter => _page.GetByRole(AriaRole.Link, new() { Name = "Active" });

    public ILocator CompletedFilter => _page.GetByRole(AriaRole.Link, new() { Name = "Completed" });

    public ILocator TodoLabel(string title)
    {
        return TodoItems.Filter(new() { HasText = title }).Locator("label");
    }

    public ILocator TodoToggle(string title)
    {
        return TodoItems.Filter(new() { HasText = title }).Locator(".toggle");
    }

    public async Task AddTodoAsync(string title)
    {
        await NewTodoInput.FillAsync(title);
        await NewTodoInput.PressAsync("Enter");
    }

    public Task<string> StoredTodosAsync()
    {
        return _page.EvaluateAsync<string>("() => localStorage.getItem('react-todos') ?? ''");
    }
}
