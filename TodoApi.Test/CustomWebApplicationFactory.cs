using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi;
using TodoApi.Models;

namespace ComponentTests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
      if (dbContextDescriptor is not null) services.Remove(dbContextDescriptor);

      services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

      using var scope = services.BuildServiceProvider().CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<TodoContext>();

      //Create initial data
      context.TodoItems.Add(new TodoItem() { Id=1, Name="uno" });
      context.TodoItems.Add(new TodoItem() { Id=2, Name="dos", IsComplete=true });
      context.TodoItems.Add(new TodoItem() { Id=3, Name="tres", IsComplete=false });

      context.SaveChanges();
    });
    
    builder.UseEnvironment("Development");
  }
}
