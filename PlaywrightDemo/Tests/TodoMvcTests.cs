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
        await TestStep("Open TodoMVC app with clean storage", OpenTodoMvcWithCleanStorageAsync);

        await TestStep("Verify empty TodoMVC page is ready for input", async () =>
        {
            await Expect(Page).ToHaveTitleAsync("React • TodoMVC");
            await Expect(TodoMvc.Heading).ToBeVisibleAsync();
            await Expect(TodoMvc.NewTodoInput).ToBeVisibleAsync();
            await Expect(TodoMvc.NewTodoInput).ToBeFocusedAsync();
            await Expect(TodoMvc.TodoItems).ToHaveCountAsync(0);
        });

        await TestStep($"Add todo '{TodoMvcTestData.BuyMilk}'", async () =>
        {
            await TodoMvc.AddTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify added todo is active", async () =>
        {
            await VerifyVisibleTodosAsync(TodoMvcTestData.BuyMilk);
            await VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify footer and input state after adding todo", async () =>
        {
            await Expect(TodoMvc.TodoCounter).ToHaveTextAsync("1 item left");
            await Expect(TodoMvc.AllFilter).ToBeVisibleAsync();
            await Expect(TodoMvc.ActiveFilter).ToBeVisibleAsync();
            await Expect(TodoMvc.CompletedFilter).ToBeVisibleAsync();
            await Expect(TodoMvc.NewTodoInput).ToHaveValueAsync(string.Empty);
        });

        await TestStep("Verify todo is saved to local storage", async () =>
        {
            await VerifyStoredTodosAsync(ExpectedActive(TodoMvcTestData.BuyMilk));
        });
    }

    [Test]
    [AllureName("Add multiple todos")]
    [AllureStory("Add multiple todos")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTmsItem("TC-002", Title = "Add multiple todos")]
    public async Task AddMultipleTodosCreatesActiveTodosInOrder()
    {
        await TestStep("Open TodoMVC app with clean storage", OpenTodoMvcWithCleanStorageAsync);

        await TestStep("Add three active todos", async () =>
        {
            await TodoMvc.AddTodosAsync(TodoMvcTestData.ThreeTodos);
        });

        await TestStep("Verify todos are displayed in insertion order", async () =>
        {
            await VerifyVisibleTodosAsync(TodoMvcTestData.ThreeTodos);
            await Expect(TodoMvc.ActiveTodoItems).ToHaveCountAsync(3);
            await Expect(TodoMvc.CompletedTodoItems).ToHaveCountAsync(0);
        });

        await TestStep("Verify footer and storage after adding multiple todos", async () =>
        {
            await Expect(TodoMvc.TodoCounter).ToHaveTextAsync("3 items left");
            await Expect(TodoMvc.NewTodoInput).ToHaveValueAsync(string.Empty);

            await VerifyStoredTodosAsync(
                ExpectedActive(TodoMvcTestData.BuyMilk),
                ExpectedActive(TodoMvcTestData.WalkDog),
                ExpectedActive(TodoMvcTestData.ReadBook));
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
            await OpenTodoMvcWithCleanStorageAsync();
            await TodoMvc.AddTodosAsync(TodoMvcTestData.ThreeTodos);
        });

        await TestStep($"Complete todo '{TodoMvcTestData.WalkDog}'", async () =>
        {
            await TodoMvc.CompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep("Verify only selected todo is completed", async () =>
        {
            await VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
            await VerifyTodoIsCompletedAsync(TodoMvcTestData.WalkDog);
            await VerifyTodoIsActiveAsync(TodoMvcTestData.ReadBook);
            await Expect(TodoMvc.ActiveTodoItems).ToHaveCountAsync(2);
            await Expect(TodoMvc.CompletedTodoItems).ToHaveCountAsync(1);
        });

        await TestStep("Verify counter, clear completed button, and storage", async () =>
        {
            await Expect(TodoMvc.TodoCounter).ToHaveTextAsync("2 items left");
            await Expect(TodoMvc.ClearCompletedButton).ToBeVisibleAsync();

            await VerifyStoredTodosAsync(
                ExpectedActive(TodoMvcTestData.BuyMilk),
                ExpectedCompleted(TodoMvcTestData.WalkDog),
                ExpectedActive(TodoMvcTestData.ReadBook));
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
            await OpenTodoMvcWithCleanStorageAsync();
            await TodoMvc.AddTodosAsync(TodoMvcTestData.ThreeTodos);
            await TodoMvc.CompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep($"Uncomplete todo '{TodoMvcTestData.WalkDog}'", async () =>
        {
            await TodoMvc.UncompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep("Verify all todos are active again", async () =>
        {
            await VerifyTodoIsActiveAsync(TodoMvcTestData.BuyMilk);
            await VerifyTodoIsActiveAsync(TodoMvcTestData.WalkDog);
            await VerifyTodoIsActiveAsync(TodoMvcTestData.ReadBook);
            await Expect(TodoMvc.ActiveTodoItems).ToHaveCountAsync(3);
            await Expect(TodoMvc.CompletedTodoItems).ToHaveCountAsync(0);
        });

        await TestStep("Verify counter, clear completed button, and storage", async () =>
        {
            await Expect(TodoMvc.TodoCounter).ToHaveTextAsync("3 items left");
            await Expect(TodoMvc.ClearCompletedButton).ToBeHiddenAsync();

            await VerifyStoredTodosAsync(
                ExpectedActive(TodoMvcTestData.BuyMilk),
                ExpectedActive(TodoMvcTestData.WalkDog),
                ExpectedActive(TodoMvcTestData.ReadBook));
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
            await OpenTodoMvcWithCleanStorageAsync();
            await TodoMvc.AddTodosAsync(TodoMvcTestData.ThreeTodos);
        });

        await TestStep($"Delete todo '{TodoMvcTestData.BuyMilk}'", async () =>
        {
            await TodoMvc.DeleteTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify only selected todo is removed", async () =>
        {
            await VerifyVisibleTodosAsync(TodoMvcTestData.WalkDog, TodoMvcTestData.ReadBook);
            await Expect(TodoMvc.TodoLabel(TodoMvcTestData.BuyMilk)).ToHaveCountAsync(0);
        });

        await TestStep("Verify counter and storage after deleting todo", async () =>
        {
            await Expect(TodoMvc.TodoCounter).ToHaveTextAsync("2 items left");

            await VerifyStoredTodosAsync(
                ExpectedActive(TodoMvcTestData.WalkDog),
                ExpectedActive(TodoMvcTestData.ReadBook));
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
            await OpenTodoMvcWithCleanStorageAsync();
            await TodoMvc.AddTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep($"Delete todo '{TodoMvcTestData.BuyMilk}'", async () =>
        {
            await TodoMvc.DeleteTodoAsync(TodoMvcTestData.BuyMilk);
        });

        await TestStep("Verify app returns to empty state", async () =>
        {
            await Expect(TodoMvc.TodoItems).ToHaveCountAsync(0);
            await Expect(TodoMvc.Main).ToHaveCountAsync(0);
            await Expect(TodoMvc.Footer).ToHaveCountAsync(0);
            await Expect(TodoMvc.NewTodoInput).ToHaveValueAsync(string.Empty);
        });

        await TestStep("Verify storage contains no todos", async () =>
        {
            await VerifyStoredTodosAsync();
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
            await OpenTodoMvcWithCleanStorageAsync();
            await TodoMvc.AddTodosAsync(TodoMvcTestData.ThreeTodos);
            await TodoMvc.CompleteTodoAsync(TodoMvcTestData.WalkDog);
        });

        await TestStep("Clear completed todos", async () =>
        {
            await Expect(TodoMvc.ClearCompletedButton).ToBeVisibleAsync();
            await TodoMvc.ClearCompletedAsync();
        });

        await TestStep("Verify completed todo is removed and active todos remain", async () =>
        {
            await VerifyVisibleTodosAsync(TodoMvcTestData.BuyMilk, TodoMvcTestData.ReadBook);
            await Expect(TodoMvc.TodoLabel(TodoMvcTestData.WalkDog)).ToHaveCountAsync(0);
            await Expect(TodoMvc.ActiveTodoItems).ToHaveCountAsync(2);
            await Expect(TodoMvc.CompletedTodoItems).ToHaveCountAsync(0);
        });

        await TestStep("Verify counter, clear completed button, and storage", async () =>
        {
            await Expect(TodoMvc.TodoCounter).ToHaveTextAsync("2 items left");
            await Expect(TodoMvc.ClearCompletedButton).ToBeHiddenAsync();

            await VerifyStoredTodosAsync(
                ExpectedActive(TodoMvcTestData.BuyMilk),
                ExpectedActive(TodoMvcTestData.ReadBook));
        });
    }
}
