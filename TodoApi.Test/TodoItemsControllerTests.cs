using System.Net.Http.Json;
using System.Text.Json;
using ComponentTests;
using Microsoft.VisualStudio.TestPlatform.TestHost;
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
        var data = await response.Content.ReadFromJsonAsync<TodoItem[]?>();
        Assert.NotNull(data);
        for (int i = 0; i < data.Length; i++) {
            var item = data[i]; 
            _output.WriteLine(item.Name);
            Assert.Equal(i + 1, item.Id);
        }
    }

    [Fact]
    public async void TestGetTodoItem()
    {
        var response = await _client.GetAsync("api/TodoItems/1");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<TodoItem?>();
        Assert.NotNull(data);
        var jsonOutput = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        _output.WriteLine("Deserialized Data (as JSON):");
        _output.WriteLine(jsonOutput);
        Assert.Equal(1, data.Id);
        Assert.Equal("uno", data.Name);
    }
}
