using Ecommerce.Catalog.Modules.Domain.Entities;

namespace Ecommerce.Catalog.Modules.Application.Services;

public class CategoryTreeService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryTreeService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// return all ids root category
    /// </summary>
    public async Task<List<Guid>> GetCategoryAndSubCategoryIdsAsync(Guid rootCategoryId, CancellationToken cancellationToken)
    {
      
        var allCategories = await _categoryRepository.GetAllActiveAsync(cancellationToken);

        var resultIds = new List<Guid>();
        CollectCategoryIdsRecursive(rootCategoryId, allCategories, resultIds);

        return resultIds;
    }

    private void CollectCategoryIdsRecursive(Guid currentId, IEnumerable<Category> allCategories, List<Guid> collectedIds)
    {
        collectedIds.Add(currentId);

   
        var children = allCategories.Where(c => c.ParentId == currentId);

        foreach (var child in children)
        {
            CollectCategoryIdsRecursive(child.Id, allCategories, collectedIds);
        }
    }
}