using Ecommerce.Modules.Users.Application.Fiatures.GetRole;
using Ecommerce.Modules.Users.Contracts.Abstractions;
using Ecommerce.Modules.Users.Contracts.Dto;

internal sealed class GetRoleQueryHandler(
    IRoleService roleService)
    : IQueryHandler<GetRoleQuery, RoleResponse>
{
    public async Task<Result<RoleResponse>> Handle(
        GetRoleQuery request,
        CancellationToken cancellationToken)
    {
        var result = await roleService.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (result.IsFailure)
        {
            return result.Error;
        }

        if (result.Value is null)
        {
            return Error.NotFound(
                "Role.NotFound",
                "Role was not found.");
        }

        return result.Value;
    }
}