namespace Ecommerce.Storefront.ApiClients.Catalogs.Models;

public sealed record CategoryListItemResponse(Guid Id, string Name,
                                            string Description, bool IsActive);


