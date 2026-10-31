using Ecommerce.Storefront.ApiClients.Currencies.Services;
using Ecommerce.Storefront.Services;

namespace Ecommerce.Storefront.ApiClients.Currencies.Endpoints;

public sealed class CurrencyEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/currency", (HttpContext context) =>
        {
            var currency = context.Request.Form["currency"].ToString();

            if (currency is not ("UAH" or "USD" or "EUR"))
            {
                return Results.BadRequest();
            }

            context.Response.Cookies.Append(
                CurrencyState.CookieName,
                currency,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(365),
                    IsEssential = true
                });

            var returnUrl = context.Request.Form["returnUrl"].ToString();

            if (string.IsNullOrWhiteSpace(returnUrl) ||
                !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative) ||
                returnUrl.StartsWith("//", StringComparison.Ordinal))
            {
                returnUrl = "/";
            }

            return Results.LocalRedirect(returnUrl);
        });
    }
}
