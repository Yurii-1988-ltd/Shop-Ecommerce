
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Admin.Authentication.Endpoints;

internal sealed class AdminLoginEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/login", HandleAsync)
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromForm] string email,
        [FromForm] string password,
        IIdentityApiClient identityApi,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return Results.Redirect("/account/login?error=invalid");
        }

        try
        {
            var response = await identityApi.LoginAsync(
                new LoginRequest(email, password),
                cancellationToken);

            if (response is null ||
                string.IsNullOrWhiteSpace(response.AccessToken))
            {
                return Results.Redirect("/account/login?error=invalid");
            }

            var principal = AuthenticationPrincipalFactory.Create(
                response.AccessToken);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return Results.Redirect("/");
        }
        catch (HttpRequestException)
        {
            return Results.Redirect("/account/login?error=unavailable");
        }
    }
}
