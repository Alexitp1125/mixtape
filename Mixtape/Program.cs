using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Mixtape.Accounts;
using Mixtape.Application.Catalog;
using Mixtape.Catalog;
using Mixtape.Components;
using Mixtape.Covers;
using Mixtape.Culture;
using Mixtape.Data;
using Mixtape.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Session: a cookie that says who signed in. It lasts 30 days from the last visit.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "mixtape-session";
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.LoginPath = PageNames.Access;
    });

builder.Services.AddMixtape(builder.Configuration);

WebApplication app = builder.Build();

// Brings the database up to date: applies the migrations that are pending (the first time, creates the tables).
await using (MixtapeDbContext dbContext = await app.Services.GetRequiredService<IDbContextFactory<MixtapeDbContext>>().CreateDbContextAsync())
{
    await dbContext.Database.MigrateAsync();
}

// Puts Alexi's catalog (Content/catalog.json) into the database: adds what is new and updates what changed.
await app.Services.GetRequiredService<CatalogManager>().SyncAsync(
    CatalogFile.Read(app.Environment),
    app.Services.GetRequiredService<CoverStore>().FileNames);

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler(PageNames.Error, createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute(PageNames.NotFound, createScopeForStatusCodePages: true);

// Language: cookie set by /culture/{culture}, otherwise the browser's language.
app.UseRequestLocalization(SupportedCultures.All);

app.UseAuthentication();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapCultureEndpoints();
app.MapAccountEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
