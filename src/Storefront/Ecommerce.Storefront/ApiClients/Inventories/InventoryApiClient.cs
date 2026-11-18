
using Ecommerce.Storefront.ApiClients.Inventories.Models;
using System.Net.Http.Json;

namespace Ecommerce.Storefront.ApiClients.Inventories;

internal sealed class InventoryApiClient(HttpClient httpClient)
    : IInventoryApiClient
{
    private const string InventoryUrl = "/inventories";

    public async Task<InventoryAvailabilityResponse?> GetAvailabilityAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"{InventoryUrl}/product/{productId}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<InventoryAvailabilityResponse>(
            cancellationToken);
    }
}
