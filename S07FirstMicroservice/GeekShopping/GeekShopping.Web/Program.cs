using GeekShopping.Web.Services;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<IProductService, ProductService>(_ =>
{
    _.BaseAddress = new Uri(builder.Configuration["ServiceURLs:ProductAPI"]);
});

builder.Services.AddHttpClient<ICartService, CartService>(_ =>
{
    _.BaseAddress = new Uri(builder.Configuration["ServiceURLs:CartAPI"]);
});

builder.Services.AddHttpClient<ICouponService, CouponService>(_ =>
{
    _.BaseAddress = new Uri(builder.Configuration["ServiceURLs:CouponAPI"]);
});

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(_ =>
{
    _.DefaultScheme = "Cookies";
    _.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies", _ => _.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc", _ =>
    {
        _.Authority = builder.Configuration["ServiceURLs:IdentityServer"];
        _.GetClaimsFromUserInfoEndpoint = true;
        _.ClientId = "geek_shopping";
        _.ClientSecret = "teste";
        _.ResponseType = "code";
        _.ClaimActions.MapJsonKey("role", "role", "role");
        _.ClaimActions.MapJsonKey("sub", "sub", "sub");
        _.TokenValidationParameters.NameClaimType = "name";
        _.TokenValidationParameters.RoleClaimType = "role";
        _.Scope.Add("geek_shopping");
        _.SaveTokens = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();