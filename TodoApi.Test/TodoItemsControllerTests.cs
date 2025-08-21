using ComponentTests;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TodoApi.Test;

public class TodoItemsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoItemsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public void TestGetTodoItems()
    {
        //var response = await _client.GetAsync("api/TodoItems");
    }
}