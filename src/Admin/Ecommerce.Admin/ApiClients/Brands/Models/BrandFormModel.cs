namespace Ecommerce.Admin.ApiClients.Brands.Models;

using System.ComponentModel.DataAnnotations;

public class BrandFormModel
{
    [Required(ErrorMessage = "Brand name is required")]
    [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}