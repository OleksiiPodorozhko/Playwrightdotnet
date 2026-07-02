using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using PlaywrightDemo.Base;
using PlaywrightDemo.TestData;

namespace PlaywrightDemo.Tests;

[TestFixture]
[AllureFeature("TodoMVC")]
public class TodoMvcTests : TodoMvcTestBase
{
    [Test]
    [AllureName("Add todo")]
    [AllureStory("Add todo")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-001", Title = "Add todo")]
    public async Task AddTodoCreatesAnActiveTodo()
    {
        await TestStep("Open TodoMVC app", OpenTodoMvcAsync);

        await TestStep("Verify empty TodoMVC page is ready for input", async () =>
        {
            await Expect(Page).ToHaveTitleAsync("React • TodoMVC");
            await Expect(TodoMvcPage.Heading).ToBeVisibleAsync();
            await Expect(TodoMvcPage.NewTodoInput).ToBeVisibleAsync();
            await Expect(TodoMvcPage.NewTodoInput).ToBeFocusedAsync();
            await Expect(TodoMvcPage.TodoItems).ToHaveCountAsync(0);
        });

        await TestStep($"Add todo '{TodoMvcTestData.BuyMilk}'", async () =>
        {
            await TodoMvcPage.AddTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify added todo is active", async () =>
        {
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.BuyMilk);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify footer and input state after adding todo", async () =>
        {
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("1 item left");
            await Expect(TodoMvcPage.AllFilter).ToBeVisibleAsync();
            await Expect(TodoMvcPage.ActiveFilter).ToBeVisibleAsync();
            await Expect(TodoMvcPage.CompletedFilter).ToBeVisibleAsync();
            await Expect(TodoMvcPage.NewTodoInput).ToHaveValueAsync(string.Empty);
        });

        await TestStep("Verify todo remains visible after reload", async () =>
        {
            await Page.ReloadAsync();
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.BuyMilk);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
        });
    }

    [Test]
    [AllureName("Add multiple todos")]
    [AllureStory("Add multiple todos")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-002", Title = "Add multiple todos")]
    public async Task AddMultipleTodosCreatesActiveTodosInOrder()
    {
        await TestStep("Open TodoMVC app", OpenTodoMvcAsync);

        await TestStep("Add three active todos", async () =>
        {
            await TodoMvcPage.AddTodosAsync(TodoMvcTestData.ThreeTodos);
        });

        await TestStep("Verify todos are displayed in insertion order", async () =>
        {
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.ThreeTodos);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(3);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(0);
        });

        await TestStep("Verify footer and persisted UI after adding multiple todos", async () =>
        {
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("3 items left");
            await Expect(TodoMvcPage.NewTodoInput).ToHaveValueAsync(string.Empty);

            await Page.ReloadAsync();
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.ThreeTodos);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(3);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("3 items left");
        });
    }

    [Test]
    [AllureName("Complete todo")]
    [AllureStory("Complete todo")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-003", Title = "Complete todo")]
    public async Task CompleteTodoMovesOnlySelectedTodoToCompleted()
    {
        await TestStep("Open TodoMVC app with three active todos", async () =>
        {
            await OpenTodoMvcAsync();
            await TodoMvcPage.AddTodosAsync(TodoMvcTestData.ThreeTodos);
        });

        await TestStep($"Complete todo '{TodoMvcTestData.WalkDog}'", async () =>
        {
            await TodoMvcPage.CompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep("Verify only selected todo is completed", async () =>
        {
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
            await TodoMvcPage.VerifyTodoIsCompletedAsync(TodoMvcTestData.WalkDog);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(2);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(1);
        });

        await TestStep("Verify counter, clear completed button, and persisted UI", async () =>
        {
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("2 items left");
            await Expect(TodoMvcPage.ClearCompletedButton).ToBeVisibleAsync();

            await Page.ReloadAsync();
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
            await TodoMvcPage.VerifyTodoIsCompletedAsync(TodoMvcTestData.WalkDog);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(2);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(1);
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("2 items left");
            await Expect(TodoMvcPage.ClearCompletedButton).ToBeVisibleAsync();
        });
    }

    [Test]
    [AllureName("Uncomplete todo")]
    [AllureStory("Uncomplete todo")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-004", Title = "Uncomplete todo")]
    public async Task UncompleteTodoReturnsItToActive()
    {
        await TestStep("Open TodoMVC app with one completed todo", async () =>
        {
            await OpenTodoMvcAsync();
            await TodoMvcPage.AddTodosAsync(TodoMvcTestData.ThreeTodos);
            await TodoMvcPage.CompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep($"Uncomplete todo '{TodoMvcTestData.WalkDog}'", async () =>
        {
            await TodoMvcPage.UncompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep("Verify all todos are active again", async () =>
        {
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.WalkDog);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(3);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(0);
        });

        await TestStep("Verify counter, clear completed button, and persisted UI", async () =>
        {
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("3 items left");
            await Expect(TodoMvcPage.ClearCompletedButton).ToBeHiddenAsync();

            await Page.ReloadAsync();
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.WalkDog);
            await TodoMvcPage.VerifyTodoIsActiveAsync(TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(3);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("3 items left");
            await Expect(TodoMvcPage.ClearCompletedButton).ToBeHiddenAsync();
        });
    }

    [Test]
    [AllureName("Delete todo")]
    [AllureStory("Delete todo")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-005", Title = "Delete todo")]
    public async Task DeleteTodoRemovesOnlySelectedTodo()
    {
        await TestStep("Open TodoMVC app with three active todos", async () =>
        {
            await OpenTodoMvcAsync();
            await TodoMvcPage.AddTodosAsync(TodoMvcTestData.ThreeTodos);
        });

        await TestStep($"Delete todo '{TodoMvcTestData.BuyMilk}'", async () =>
        {
            await TodoMvcPage.DeleteTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify only selected todo is removed", async () =>
        {
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.WalkDog, TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.TodoLabel(TodoMvcTestData.BuyMilk)).ToHaveCountAsync(0);
        });

        await TestStep("Verify counter and persisted UI after deleting todo", async () =>
        {
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("2 items left");

            await Page.ReloadAsync();
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.WalkDog, TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.TodoLabel(TodoMvcTestData.BuyMilk)).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("2 items left");
        });
    }

    [Test]
    [AllureName("Delete last todo")]
    [AllureStory("Delete todo")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-006", Title = "Delete last todo")]
    public async Task DeleteLastTodoReturnsAppToEmptyState()
    {
        await TestStep("Open TodoMVC app with one active todo", async () =>
        {
            await OpenTodoMvcAsync();
            await TodoMvcPage.AddTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep($"Delete todo '{TodoMvcTestData.BuyMilk}'", async () =>
        {
            await TodoMvcPage.DeleteTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify app returns to empty state", async () =>
        {
            await Expect(TodoMvcPage.TodoItems).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.Main).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.Footer).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.NewTodoInput).ToHaveValueAsync(string.Empty);
        });

        await TestStep("Verify empty state remains after reload", async () =>
        {
            await Page.ReloadAsync();
            await Expect(TodoMvcPage.TodoItems).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.Main).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.Footer).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.NewTodoInput).ToHaveValueAsync(string.Empty);
        });
    }

    [Test]
    [AllureName("Clear completed")]
    [AllureStory("Clear completed")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-007", Title = "Clear completed")]
    public async Task ClearCompletedRemovesOnlyCompletedTodos()
    {
        await TestStep("Open TodoMVC app with active and completed todos", async () =>
        {
            await OpenTodoMvcAsync();
            await TodoMvcPage.AddTodosAsync(TodoMvcTestData.ThreeTodos);
            await TodoMvcPage.CompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep("Clear completed todos", async () =>
        {
            await Expect(TodoMvcPage.ClearCompletedButton).ToBeVisibleAsync();
            await TodoMvcPage.ClearCompletedAsync();
        });

        await TestStep("Verify completed todo is removed and active todos remain", async () =>
        {
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.BuyMilk, TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.TodoLabel(TodoMvcTestData.WalkDog)).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(2);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(0);
        });

        await TestStep("Verify counter, clear completed button, and persisted UI", async () =>
        {
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("2 items left");
            await Expect(TodoMvcPage.ClearCompletedButton).ToBeHiddenAsync();

            await Page.ReloadAsync();
            await TodoMvcPage.VerifyVisibleTodosAsync(TodoMvcTestData.BuyMilk, TodoMvcTestData.ReadBook);
            await Expect(TodoMvcPage.TodoLabel(TodoMvcTestData.WalkDog)).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.ActiveTodoItems).ToHaveCountAsync(2);
            await Expect(TodoMvcPage.CompletedTodoItems).ToHaveCountAsync(0);
            await Expect(TodoMvcPage.TodoCounter).ToHaveTextAsync("2 items left");
            await Expect(TodoMvcPage.ClearCompletedButton).ToBeHiddenAsync();
        });
    }
}
