using System.Text.Json;
using System.Text.Json.Serialization;
using Ecommerce.Admin;
using Ecommerce.Admin.ApiClients.Shippings;
using Ecommerce.Admin.ApiClients.Shippings.Api;
using Ecommerce.Admin.Authentication.Endpoints;
using Microsoft.AspNetCore.Components.Authorization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Razor Components & UI
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddSortable();
        builder.Services.AddMudServices();

        // API Clients
        const string apiBaseAddress = "https://localhost:7125";
        var baseUri = new Uri(apiBaseAddress);

        builder.Services.AddHttpClient<ICategoryApiClient, CategoryApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<ICatalogApiClient, CatalogApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient("Catalog", client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IBrandApiClient, BrandsApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IImageApiClient, ImageApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<ICartApiClient, CartApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IOrderApiClient, OrderApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IInventoryApiClient, InventoryApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IUserApiClient, UserApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IRoleApiClient, RoleApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<ICouponApiClient, CouponApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IIdentityApiClient, IdentityApiClient>(client => client.BaseAddress = baseUri);
        builder.Services.AddHttpClient<IShippingApiClient, ShippingApiClient>(client => client.BaseAddress = baseUri);

        // JSON options
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: true));
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
        });

        // Auth & Services
        builder.Services.AddScoped<AuthenticationService>();
        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();

        // Modules
        builder.Services.AddModule<AdminModule>(builder.Configuration);

        var app = builder.Build();

        // Pipeline Order
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        // Endpoints
        app.MapModules();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Diagnostic code to catch ambiguous routes in output console
        // Додайте вгору файлу, якщо не підтягнулося:
        // using Microsoft.Extensions.DependencyInjection;

        // Diagnose duplicate routes
        var dataSources = app.Services.GetServices<EndpointDataSource>();
        var duplicateRoutes = dataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.RoutePattern.RawText is not null)
            .GroupBy(endpoint => endpoint.RoutePattern.RawText, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1);

        foreach (var group in duplicateRoutes)
        {
            Console.WriteLine($"DUPLICATE ROUTE: {group.Key}");
            foreach (var endpoint in group)
            {
                Console.WriteLine($"  -> {endpoint.DisplayName}");
            }
        }

        app.Run();
    }
}