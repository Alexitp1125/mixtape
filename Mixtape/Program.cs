using Microsoft.EntityFrameworkCore;
using Mixtape.Components;
using Mixtape.Culture;
using Mixtape.Data;
using Mixtape.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMixtape(builder.Configuration);

WebApplication app = builder.Build();

// Brings the database up to date: applies the migrations that are pending (the first time, creates the tables).
await using (MixtapeDbContext dbContext = await app.Services.GetRequiredService<IDbContextFactory<MixtapeDbContext>>().CreateDbContextAsync())
{
    await dbContext.Database.MigrateAsync();
}

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapCultureEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
