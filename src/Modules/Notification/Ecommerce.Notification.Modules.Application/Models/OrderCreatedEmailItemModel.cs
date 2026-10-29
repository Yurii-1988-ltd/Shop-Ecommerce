using System.Globalization;

namespace Ecommerce.Notification.Modules.Application.Models;

public sealed record OrderCreatedEmailItemModel(
    string ProductName,
    string Sku,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal)
{
    public string FormattedUnitPrice =>
        UnitPrice.ToString("N2", CultureInfo.GetCultureInfo("uk-UA"));

    public string FormattedLineTotal =>
        LineTotal.ToString("N2", CultureInfo.GetCultureInfo("uk-UA"));
}