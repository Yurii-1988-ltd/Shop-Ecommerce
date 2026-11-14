

using Ecommerce.Admin.ApiClients.Shippings.Models;

namespace Ecommerce.Admin.ApiClients.Shippings.Api;

internal sealed class ShippingApiClient(HttpClient httpClient)
    : IShippingApiClient
{
    private const string ShippingUrl = "/shippings";

    public async Task<PagedResult<ShippingResponse>?> GetAllAsync(
        int page,
        int pageSize,
        string? status = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{ShippingUrl}?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(status))
            url += $"&status={Uri.EscapeDataString(status)}";

        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return await httpClient.GetFromJsonAsync<PagedResult<ShippingResponse>>(
            url,
            cancellationToken);
    }
    public async Task<ShippingResponse?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ShippingResponse>(
            $"{ShippingUrl}/{id}",
            cancellationToken);
    }

    public async Task<List<ShipmentTrackingEventResponse>?> GetTrackingEventsAsync(Guid shippingId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<List<ShipmentTrackingEventResponse>>(
            $"{ShippingUrl}/{shippingId}/tracking-events",
            cancellationToken);

    }
    public Task StartProcessingAsync(Guid id, CancellationToken cancellationToken = default)
    => PostAsync($"{ShippingUrl}/{id}/start-processing", cancellationToken);

    public Task MarkAsReadyForShipmentAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/ready-for-shipment", cancellationToken);

    public Task MarkAsShippedAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/ship", cancellationToken);

    public Task MarkInTransitAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/in-transit", cancellationToken);

    public Task MarkOutForDeliveryAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/out-for-delivery", cancellationToken);

    public Task MarkAsDeliveredAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/deliver", cancellationToken);

    public Task MarkAsFailedAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/fail", cancellationToken);

    public Task MarkAsReturnedAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/return", cancellationToken);

    public Task CancelAsync(Guid id, CancellationToken cancellationToken = default)
        => PostAsync($"{ShippingUrl}/{id}/cancel", cancellationToken);

    private async Task PostAsync(string url, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsync(
            url,
            null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}