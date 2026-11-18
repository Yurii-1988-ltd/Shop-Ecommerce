
using Ecommerce.Admin.ApiClients.Inventories.Responses;
using Ecommerce.Admin.Components.Pages.Inventories.Dialogs;
using Ecommerce.Admin.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Ecommerce.Admin.Components.Pages.Inventories;

public partial class Inventories
{
    [Inject]
    public IJSRuntime JS { get; set; } = default!;
    private List<InventoryReportItemResponse> _inventories = [];
    private bool _isLoading;
    private bool _isExporting;

    protected override async Task OnInitializedAsync()
    {
        await LoadInventory();
    }

    private async Task LoadInventory()
    {
        _isLoading = true;

        try
        {
            var result = await InventoryApi.GetInventoryReportAsync();
            _inventories = result?.ToList() ?? [];
        }
        catch (ApiException ex)
        {
            Snackbar.Add(
                ex.Description,
                Severity.Error);
        }
        catch (HttpRequestException)
        {
            Snackbar.Add(
                "Failed to load inventory.",
                Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    // Replenish
    private Task Replenish(InventoryReportItemResponse inventory) =>
        ExecuteQuantityDialogAsync(
            title: "Replenish Inventory",
            actionText: "Replenish",
            icon: Icons.Material.Filled.Add,
            inventory: inventory,
            quantityLabel: "On Hand",
            currentQuantity: inventory.OnHandQuantity,
            apiAction: (id, q) => InventoryApi.ReplenishStockAsync(id, q));

    // Deduct
    private Task Deduct(InventoryReportItemResponse inventory) =>
        ExecuteQuantityDialogAsync(
            title: "Deduct Inventory",
            actionText: "Deduct",
            icon: Icons.Material.Filled.Remove,
            inventory: inventory,
            quantityLabel: "On Hand",
            currentQuantity: inventory.OnHandQuantity,
            apiAction: (id, q) => InventoryApi.DeductAsync(id, q));

    // Reserve
    private Task Reserve(InventoryReportItemResponse inventory) =>
        ExecuteQuantityDialogAsync(
            title: "Reserve Inventory",
            actionText: "Reserve",
            icon: Icons.Material.Filled.Lock,
            inventory: inventory,
            quantityLabel: "Available",
            currentQuantity: inventory.AvailableQuantity,
            apiAction: (id, q) => InventoryApi.ReserveStockAsync(id, q));

    // Cancel Reservation
    private Task CancelReservation(InventoryReportItemResponse inventory) =>
        ExecuteQuantityDialogAsync(
            title: "Cancel Reservation",
            actionText: "Cancel Reservation",
            icon: Icons.Material.Filled.LockOpen,
            inventory: inventory,
            quantityLabel: "Reserved",
            currentQuantity: inventory.ReservedQuantity,
            apiAction: (id, q) => InventoryApi.CancelReservationAsync(id, q));

    // Commit Reservation
    private Task CommitReservation(InventoryReportItemResponse inventory) =>
        ExecuteQuantityDialogAsync(
            title: "Commit Reservation",
            actionText: "Commit",
            icon: Icons.Material.Filled.CheckCircle,
            inventory: inventory,
            quantityLabel: "Reserved",
            currentQuantity: inventory.ReservedQuantity,
            apiAction: (id, q) => InventoryApi.CommitReservationAsync(id, q));

    private async Task ExecuteQuantityDialogAsync(
        string title,
        string actionText,
        string icon,
        InventoryReportItemResponse inventory,
        string quantityLabel,
        int currentQuantity,
        Func<Guid, int, Task> apiAction)
    {
        var parameters = new DialogParameters
        {
            { nameof(InventoryQuantityDialog.Title), title },
            { nameof(InventoryQuantityDialog.ActionText), actionText },
            { nameof(InventoryQuantityDialog.Icon), icon },
            { nameof(InventoryQuantityDialog.SKU), inventory.SKU },
            { nameof(InventoryQuantityDialog.QuantityLabel), quantityLabel },
            { nameof(InventoryQuantityDialog.CurrentQuantity), currentQuantity }
        };

        var dialog = await DialogService.ShowAsync<InventoryQuantityDialog>(
            title,
            parameters);

        var result = await dialog.Result;

        if (result is null || result.Canceled || result.Data is not int quantity)
            return;

        try
        {
            await apiAction(inventory.InventoryItemId, quantity);

            Snackbar.Add(
                $"{actionText} completed successfully.",
                Severity.Success);

            await LoadInventory();
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Description, Severity.Warning);
        }
        catch (HttpRequestException)
        {
            Snackbar.Add(
                $"Failed to {actionText.ToLowerInvariant()} inventory.",
                Severity.Error);
        }
    }

    // Excel export
    private Task ExportExcel() =>
        ExportFileAsync(
            "inventories.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ct => InventoryApi.ExportExcelAsync(ct));

    // PDF export
    private Task ExportPdf() =>
        ExportFileAsync(
            "inventories.pdf",
            "application/pdf",
            ct => InventoryApi.ExportPdfAsync(ct));

    private async Task ExportFileAsync(
        string fileName,
        string contentType,
        Func<CancellationToken, Task<byte[]>> exportFunc)
    {
        if (_isExporting)
            return;

        _isExporting = true;

        try
        {
            var file = await exportFunc(CancellationToken.None);

            await JS.InvokeVoidAsync(
                "downloadFile",
                fileName,
                contentType,
                file);
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                $"Failed to export {fileName}: {ex.Message}",
                Severity.Error);
        }
        finally
        {
            _isExporting = false;
        }
    }
}
