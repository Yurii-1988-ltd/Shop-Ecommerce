

using Ecommerce.Storefront.ApiClients.Currencies.Endpoints;
using Ecommerce.Storefront.ApiClients.Currencies.Services;
using Ecommerce.Storefront.Services;

var builder = WebApplication.CreateBuilder(args);

// HTTP context + authentication token handler
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AccessTokenHandler>();

// API clients

builder.Services.AddHttpClient<ICatalogApiClient, CatalogApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7125");
});

builder.Services.AddHttpClient<ICartApiClient, CartApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7125");
})
.AddHttpMessageHandler<AccessTokenHandler>();

builder.Services.AddHttpClient<IIdentityApiClient, IdentityApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7125");
})
.AddHttpMessageHandler<AccessTokenHandler>();

builder.Services.AddHttpClient<IInventoryApiClient, InventoryApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7125");
});
builder.Services.AddHttpClient<ICurrencyApiClient, CurrencyApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7125");
});

builder.Services.AddHttpClient<IOrderApiClient, OrderApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7125");
})
.AddHttpMessageHandler<AccessTokenHandler>();

// Application services
builder.Services.AddScoped<CartState>();
builder.Services.AddScoped<ICartMergeService, CartMergeService>();

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Authentication
builder.Services.AddStorefrontAuthentication();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CurrencyState>();

var app = builder.Build();

// HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseMiddleware<GuestCartMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

new LoginEndpoint().MapEndpoint(app);
new LogoutEndpoint().MapEndpoint(app);
new RegisterEndpoint().MapEndpoint(app);
new CurrencyEndpoint().MapEndpoint(app);
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();