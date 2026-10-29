using System.Globalization;

namespace Ecommerce.Notification.Modules.Application.Models;

public sealed record OrderCreatedEmailModel(
    string OrderNumber,
    decimal TotalAmount,
    string Currency,
    IReadOnlyCollection<OrderCreatedEmailItemModel> Items)
{
    public string FormattedTotalAmount =>
        TotalAmount.ToString(
            "N2",
            CultureInfo.GetCultureInfo("uk-UA"));
}