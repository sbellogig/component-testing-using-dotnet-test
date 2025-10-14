using System.Net.Http.Json;
using System.Text.Json;
using ComponentTests;
using Humanizer;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NuGet.ContentModel;
using TodoApi.Models;
using Xunit.Abstractions;

namespace TodoApi.Test;

public class TodoItemsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public TodoItemsControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async void TestGetTodoItems()
    {
        var response = await _client.GetAsync("api/TodoItems");
        response.EnsureSuccessStatusCode();
        TodoItem[]? data = await response.Content.ReadFromJsonAsync<TodoItem[]?>();
        Assert.NotNull(data);
        for (int i = 0; i < data.Length; i++)
        {
            var item = data[i];
            _output.WriteLine(item.Name);
            Assert.Equal(i + 1, item.Id);
        }
        Assert.Equal("uno", data[0].Name);
        Assert.Equal("dos", data[1].Name);
        Assert.False(data[0].IsComplete);
        Assert.True(data[1].IsComplete);
    }

    [Fact]
    public async void TestGetTodoItem()
    {
        var response = await _client.GetAsync("api/TodoItems/1");
        response.EnsureSuccessStatusCode();
        TodoItem? data = await response.Content.ReadFromJsonAsync<TodoItem?>();
        Assert.NotNull(data);
        var jsonOutput = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine("Deserialized Data (as JSON):");
        _output.WriteLine(jsonOutput);
        Assert.Equal(1, data.Id);
        Assert.Equal("uno", data.Name);
    }

    [Fact]
    public async void TestPutTodoItem()
    {
        TodoItem todoItem = new TodoItem { Id = 1, Name = "otro", IsComplete = true };
        String jsonString = JsonSerializer.Serialize(todoItem, new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(jsonString);
        var response = await _client.PutAsJsonAsync("api/TodoItems/" + todoItem.Id, jsonString);
        response.EnsureSuccessStatusCode();
        TodoItem? data = await response.Content.ReadFromJsonAsync<TodoItem?>();
        Assert.NotNull(data);
        Assert.Equal(todoItem.Id, data.Id);
        Assert.Equal(todoItem.Name, data.Name);
        Assert.Equal(todoItem.IsComplete, data.IsComplete);
    }

    [Fact]
    public async void TestPostTodoItem()
    {
        TodoItem todoItem = new TodoItem { Id = 3, Name = "tres", IsComplete = true };
        String jsonString = JsonSerializer.Serialize(todoItem, new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine(jsonString);
        var response = await _client.PostAsJsonAsync("api/TodoItems", jsonString);
        response.EnsureSuccessStatusCode();
        TodoItem? data = await response.Content.ReadFromJsonAsync<TodoItem?>();
        Assert.NotNull(data);
        Assert.Equal(todoItem.Id, data.Id);
        Assert.Equal(todoItem.Name, data.Name);
        Assert.Equal(todoItem.IsComplete, data.IsComplete);
    }

    [Fact]
    public async void TestDeleteTodoItem()
    {
        TestPostTodoItem();
        TodoItem todoItem = new TodoItem { Id = 3, Name = "tres", IsComplete = true };
        //TodoItem? data = await _client.DeleteFromJsonAsync<TodoItem>("api/TodoItems/" + todoItem.Id);
        var response = await _client.DeleteAsync("api/TodoItems/" + todoItem.Id);
        response.EnsureSuccessStatusCode();
        TodoItem? data = await response.Content.ReadFromJsonAsync<TodoItem?>();
        Assert.NotNull(data);
        Assert.Equal(todoItem.Id, data.Id);
        Assert.Equal(todoItem.Name, data.Name);
        Assert.Equal(todoItem.IsComplete, data.IsComplete);
    }
}
