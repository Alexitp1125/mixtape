using Mixtape.Components;
using Mixtape.Culture;
using Mixtape.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMixtape();

WebApplication app = builder.Build();

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
