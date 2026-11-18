
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Ecommerce.Admin.Authentication.Endpoints;

internal sealed class AdminLogoutEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/account/logout",
                (Delegate)HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext)
    {
        await httpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Results.Redirect("/login");
    }
}
