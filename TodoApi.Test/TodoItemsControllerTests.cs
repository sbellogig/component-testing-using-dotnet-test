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
        TodoItemDTO[]? data = await response.Content.ReadFromJsonAsync<TodoItemDTO[]?>();
        Assert.NotNull(data);
        for (int i = 0; i < data.Length; i++)
        {
            var item = data[i];
            _output.WriteLine(item.Name);
        }
        Assert.Equal("dos", data[1].Name);
        Assert.True(data[1].IsComplete);
    }

    [Fact]
    public async void TestGetTodoItem()
    {
        TodoItemDTO todoItem = new TodoItemDTO { Id = 2, Name = "dos", IsComplete = true };
        var response = await _client.GetAsync("api/TodoItems/" + todoItem.Id);
        response.EnsureSuccessStatusCode();
        TodoItemDTO? data = await response.Content.ReadFromJsonAsync<TodoItemDTO?>();
        Assert.NotNull(data);
        var jsonOutput = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine("Deserialized Data (as JSON):");
        _output.WriteLine(jsonOutput);
        Assert.Equal(todoItem.Id, data.Id);
        Assert.Equal(todoItem.Name, data.Name);
        Assert.Equal(todoItem.IsComplete, data.IsComplete);
    }

    [Fact]
    public async void TestPutTodoItem()
    {
        TodoItemDTO todoItem = new TodoItemDTO { Id = 1, Name = "otro", IsComplete = true };
        var response = await _client.PutAsJsonAsync("api/TodoItems/" + todoItem.Id, todoItem);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async void TestPostTodoItem()
    {
        TodoItemDTO todoItem = new TodoItemDTO { Id=4, Name = "cuatro", IsComplete = true };
        var response = await _client.PostAsJsonAsync("api/TodoItems", todoItem);
        response.EnsureSuccessStatusCode();
        TodoItemDTO? data = await response.Content.ReadFromJsonAsync<TodoItemDTO?>();
        Assert.NotNull(data);
        Assert.Equal(todoItem.Id, data.Id);
        Assert.Equal(todoItem.Name, data.Name);
        Assert.Equal(todoItem.IsComplete, data.IsComplete);
    }

    [Fact]
    public async void TestDeleteTodoItem()
    {
        TodoItemDTO todoItem = new TodoItemDTO { Id = 3};
         _output.WriteLine("Deleting: " + todoItem.Id);
        var response = await _client.DeleteAsync("api/TodoItems/" + todoItem.Id);
        response.EnsureSuccessStatusCode();
        response = await _client.GetAsync("api/TodoItems/" + todoItem.Id);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
