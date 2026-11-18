
using Ecommerce.Admin.ApiClients.Orders.Enums;
using Ecommerce.Admin.ApiClients.Orders.Models;
using Ecommerce.Admin.Components.Pages.Orders.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ecommerce.Admin.Components.Pages.Orders;

public partial class Details
{
    [Parameter]
    public string Slug { get; set; } = string.Empty;

    private OrderResponse? _order;

    private bool IsReadOnly =>
        _order is null || _order.Status != OrderStatus.Draft;

    protected override async Task OnParametersSetAsync()
    {
        await LoadOrderAsync();
    }

    private async Task LoadOrderAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Slug))
            {
                Navigation.NavigateTo("/orders");
                return;
            }

            _order = await OrderApi.GetBySlugAsync(Slug);

            if (_order is null)
            {
                Navigation.NavigateTo("/orders");
            }
        }
        catch (Exception ex)
        {
            _order = null;

            Snackbar.Add(
                $"Error loading order: {ex.Message}",
                Severity.Error);

            Navigation.NavigateTo("/orders");
        }
    }

    private async Task OpenSubmitDialogAsync()
    {
        if (_order is null)
            return;

        var parameters = new DialogParameters<SubmitOrderDialog>
        {
            { x => x.OrderNumber, _order.OrderNumber }
        };

        var dialog = await DialogService.ShowAsync<SubmitOrderDialog>(
            "Submit order",
            parameters);

        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await SubmitOrderAsync();
        }
    }

    private async Task SubmitOrderAsync()
    {
        if (_order is null)
            return;

        try
        {
            await OrderApi.SubmitAsync(_order.Id);

            Snackbar.Add(
                "Order submitted successfully.",
                Severity.Success);

            await LoadOrderAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task OpenChangeStatusDialogAsync()
    {
        if (_order is null)
            return;

        var parameters = new DialogParameters<ChangeOrderStatusDialog>
        {
            { x => x.OrderNumber, _order.OrderNumber },
            { x => x.CurrentStatus, _order.Status }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<ChangeOrderStatusDialog>(
            "Change Status",
            parameters,
            options);

        var result = await dialog.Result;

        if (result is null ||
            result.Canceled ||
            result.Data is not OrderStatus newStatus)
        {
            return;
        }

        await ChangeStatusAsync(newStatus);
    }

    private async Task ChangeStatusAsync(OrderStatus newStatus)
    {
        if (_order is null)
            return;

        try
        {
            await OrderApi.ChangeStatusAsync(
                _order.Id,
                new ChangeStatusRequest(newStatus));

            Snackbar.Add(
                $"Status updated to {newStatus}.",
                Severity.Success);

            await LoadOrderAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task OpenForceChangeStatusDialogAsync()
    {
        if (_order is null)
            return;

        var parameters = new DialogParameters<ForceChangeStatusDialog>
        {
            { x => x.OrderNumber, _order.OrderNumber },
            { x => x.CurrentStatus, _order.Status }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<ForceChangeStatusDialog>(
            "Force Change Status",
            parameters,
            options);

        var result = await dialog.Result;

        if (result is null ||
            result.Canceled ||
            result.Data is not ForceChangeStatusDialog.ForceChangeStatusResult data)
        {
            return;
        }

        await ForceChangeStatusAsync(data);
    }

    private async Task ForceChangeStatusAsync(
        ForceChangeStatusDialog.ForceChangeStatusResult data)
    {
        if (_order is null)
            return;

        try
        {
            await OrderApi.ForceChangeStatusAsync(
                _order.Id,
                new ForceChangeStatusRequest(
                    data.Status,
                    data.Reason));

            Snackbar.Add(
                "Order status force-changed successfully.",
                Severity.Success);

            await LoadOrderAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task OpenAddItemDialogAsync()
    {
        if (_order is null || IsReadOnly)
            return;

        try
        {
            var products = await CatalogApi.GetProductsAsync(
                page: 1,
                pageSize: 100);

            var parameters = new DialogParameters<OrderAddItemDialog>
            {
                { x => x.Products, products?.Items }
            };

            var dialog = await DialogService.ShowAsync<OrderAddItemDialog>(
                "Add Product",
                parameters);

            var result = await dialog.Result;

            if (result is null ||
                result.Canceled ||
                result.Data is not OrderAddItemDialogResult data)
            {
                return;
            }

            await OrderApi.AddOrderItemAsync(
                _order.Id,
                new AddOrderItemRequest(
                    data.ProductId,
                    data.Quantity));

            Snackbar.Add("Product added.", Severity.Success);

            await LoadOrderAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task ChangeItemQuantityAsync(
        (Guid OrderItemId, ChangeOrderItemQuantityRequest Request) model)
    {
        if (_order is null || IsReadOnly)
            return;

        try
        {
            await OrderApi.ChangeOrderItemQuantityAsync(
                _order.Id,
                model.OrderItemId,
                model.Request);

            Snackbar.Add("Quantity updated.", Severity.Success);

            await LoadOrderAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task RemoveOrderItemAsync(Guid orderItemId)
    {
        if (_order is null || IsReadOnly)
            return;

        try
        {
            await OrderApi.RemoveOrderItemAsync(
                _order.Id,
                orderItemId);

            Snackbar.Add("Item removed.", Severity.Success);

            await LoadOrderAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}
