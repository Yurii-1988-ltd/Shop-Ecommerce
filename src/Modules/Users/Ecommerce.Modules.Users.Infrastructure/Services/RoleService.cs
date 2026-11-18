
using Ecommerce.Modules.Users.Application.Mapper;
using Ecommerce.Modules.Users.Contracts.Dto;

namespace Ecommerce.Modules.Users.Infrastructure.Services;

internal sealed class RoleService(IRoleRepository repository) : IRoleService
{
    public Task<Result<Guid>> CreateAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var result = Role.Create(name);

        if (result.IsFailure)
        {
            return Task.FromResult<Result<Guid>>(result.Error);
        }

        repository.Add(result.Value);

        return Task.FromResult<Result<Guid>>(result.Value.Id);
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByIdAsync(id, cancellationToken);

        if (role is null)
        {
            return RolesErrors.NotFound(id);
        }

        repository.Remove(role);

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<RoleResponse>>> GetAllAsync(
     CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetAllAsync(cancellationToken);

        IReadOnlyList<RoleResponse> response = roles
            .Select(x => x.ToResponse())
            .ToList();

        return Result.Success(response);
    }

    public async Task<Result<RoleResponse?>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByIdAsync(id, cancellationToken);

        if (role is null)
        {
            return RolesErrors.NotFound(id);
        }

        return role.ToResponse();
    }

    public async Task<Result<Guid>> UpdateAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByIdAsync(id, cancellationToken);

        if (role is null)
        {
            return RolesErrors.NotFound(id);
        }

        var result = role.Update(name);

        if (result.IsFailure)
        {
            return result.Error;
        }

        repository.Update(role);

        return role.Id;
    }
}
