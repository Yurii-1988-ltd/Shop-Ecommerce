

using Ecommerce.Catalog.Modules.Domain.Errors;
using Ecommerce.Domain.Domain;

namespace Ecommerce.Catalog.Modules.Domain.Entities;

public sealed class Category : Entity
{
    #region Fields and constructors

    public string Name { get; set; } = string.Empty;
    public string? Description { get; private set; }
    public  Guid? ParentId  { get;private set; }
    public Category? Parent { get;private set; }

    //Sub categories
    private readonly List<Category> _subCategories = new();
    public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

    public bool IsActive { get;private set; }
    public DateTime CreatedAtUtc { get;private set; }
    public DateTime? UpdateAtUtc  { get;private set; }

    private Category() { }
    #endregion
    // static factory

    public static Result<Category>Create(string name, string?description = null,Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CategoryErrors.NameIsRequired;
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            ParentId  = parentId,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };      
    }
    public Result Update(string name, string?description = null, Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CategoryErrors.NameIsRequired;
        if (parentId.HasValue && parentId.Value == Id)
            return CategoryErrors.InvalidParentCategory;
        Name = name;
        Description = description;
        ParentId = parentId;
        UpdateAtUtc = DateTime.UtcNow;
        return Result.Success();
    }
    public void AddSubCategory(Category subCategory)
    {
        if(!_subCategories.Contains(subCategory))
        {
            _subCategories.Add(subCategory);
            subCategory.ParentId = Id;
            subCategory.Parent = this;
            UpdateAtUtc = DateTime.UtcNow;

        }
    }

}
