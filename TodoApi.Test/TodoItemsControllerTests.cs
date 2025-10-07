using System.Net.Http.Json;
using ComponentTests;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TodoApi.Models;

namespace TodoApi.Test;

public class TodoItemsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoItemsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async void TestGetTodoItems()
    {
        var response = await _client.GetAsync("api/TodoItems");
        Console.Write(response.Content);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<TodoItem[]?>();
        Console.Write(data);
        Assert.NotNull(data);
        for (int i = 0; i < data.Length; i++) {
            var item = data[i]; 
            Assert.Equal(i + 1, item.Id);
        }
    }

    [Fact]
    public async void TestGetTodoItem()
    {
        var response = await _client.GetAsync("api/TodoItems?id=1");
        Console.Write(response);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<TodoItem?>();
        Console.Write(data);
        Assert.NotNull(data);
        Assert.Equal(1, data.Id);
        Assert.Equal("uno", data.Name);
    }
}
