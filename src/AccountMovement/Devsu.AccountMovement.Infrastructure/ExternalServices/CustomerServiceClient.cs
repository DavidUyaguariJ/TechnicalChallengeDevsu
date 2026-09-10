using System.Net;
using System.Net.Http.Json;
using Devsu.AccountMovement.Application.Ports;
using Devsu.Shared.Contracts;

namespace Devsu.AccountMovement.Infrastructure.ExternalServices;

public class CustomerServiceClient : ICustomerServiceClient
{
    private readonly HttpClient _httpClient;

    public CustomerServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerSummary?> GetByIdAsync(long customerId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"clientes/{customerId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CustomerSummary>(cancellationToken);
    }
}
