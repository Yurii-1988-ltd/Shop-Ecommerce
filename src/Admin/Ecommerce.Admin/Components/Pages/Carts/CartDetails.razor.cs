
using Ecommerce.Admin.ApiClients.Carts.Contracts;
using Ecommerce.Admin.Components.Pages.Carts.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ecommerce.Admin.Components.Pages.Carts;

public partial class CartDetails : ComponentBase
{
    [Parameter]
    public Guid CartId { get; set; }

    private CartResponse? _cart;

    private string _couponCode = string.Empty;
    private string? _couponError;
    private string? _error;

    private bool _loading;
    private bool _couponLoading;
    private bool _processing;

    private Guid CurrentCustomerId =>
        _cart?.CustomerId
        ?? throw new InvalidOperationException(
            "CustomerId was not found in the cart.");

    protected override async Task OnParametersSetAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loading = true;
        _error = null;

        try
        {
            _cart = await CartApi.GetByIdAsync(CartId);
        }
        catch (HttpRequestException ex)
        {
            _cart = null;
            _error = ex.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task RemoveItem(Guid productId)
    {
        if (_processing)
            return;

        _processing = true;

        try
        {
            await CartApi.RemoveItemAsync(
                CurrentCustomerId,
                productId);

            await LoadAsync();
        }
        catch (HttpRequestException ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _processing = false;
        }
    }

    private async Task ClearCart()
    {
        if (_processing)
            return;

        _processing = true;

        try
        {
            await CartApi.ClearAsync(CurrentCustomerId);
            await LoadAsync();
        }
        catch (HttpRequestException ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _processing = false;
        }
    }

    private async Task ChangeQuantity(CartItemResponse item)
    {
        var parameters = new DialogParameters
        {
            {
                nameof(ChangeQuantityDialog.CurrentQuantity),
                item.Quantity
            }
        };

        var dialog = await DialogService.ShowAsync<ChangeQuantityDialog>(
            "Change Quantity",
            parameters);

        var result = await dialog.Result;

        if (result is null || result.Canceled)
            return;

        if (result.Data is not int quantity || quantity <= 0)
            return;

        if (_processing)
            return;

        _processing = true;

        try
        {
            await CartApi.ChangeQuantityAsync(
                CurrentCustomerId,
                item.ProductId,
                new ChangeCartItemQuantityRequest(
                    CurrentCustomerId,
                    item.ProductId,
                    quantity));

            await LoadAsync();
        }
        catch (HttpRequestException ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _processing = false;
        }
    }

    private async Task ApplyCoupon()
    {
        if (string.IsNullOrWhiteSpace(_couponCode) || _couponLoading)
            return;

        _couponError = null;
        _couponLoading = true;

        try
        {
            await CartApi.ApplyCouponAsync(
                CurrentCustomerId,
                new ApplyCouponRequest(_couponCode.Trim()));

            _couponCode = string.Empty;

            await LoadAsync();
        }
        catch (HttpRequestException ex)
        {
            _couponError = ex.Message;
        }
        finally
        {
            _couponLoading = false;
        }
    }

    private async Task RemoveCoupon()
    {
        if (_couponLoading)
            return;

        _couponLoading = true;
        _couponError = null;

        try
        {
            await CartApi.RemoveCouponAsync(CurrentCustomerId);
            await LoadAsync();
        }
        catch (HttpRequestException ex)
        {
            _couponError = ex.Message;
        }
        finally
        {
            _couponLoading = false;
        }
    }
}
