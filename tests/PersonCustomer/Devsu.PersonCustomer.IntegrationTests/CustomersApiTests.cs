using System.Net;
using System.Net.Http.Json;
using Devsu.PersonCustomer.Application.Dtos;
using Devsu.PersonCustomer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.PersonCustomer.IntegrationTests;

public class CustomersApiTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CustomersApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateGetAndDeleteCustomer_FullHttpFlow_PersistsAndRemovesCustomer()
    {
        var request = new CreateCustomerRequest(
            "Jose Lema",
            "Male",
            35,
            Guid.NewGuid().ToString("N")[..10],
            "Otavalo sn y principal",
            "098254785",
            "1234");

        var createResponse = await _client.PostAsJsonAsync("clientes", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(created);
        Assert.Equal("Jose Lema", created!.Name);
        Assert.True(created.Status);

        var getResponse = await _client.GetAsync($"clientes/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.Equal(request.Identification, fetched!.Identification);

        var deleteResponse = await _client.DeleteAsync($"clientes/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDeleteResponse = await _client.GetAsync($"clientes/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDeleteResponse.StatusCode);
    }
}
