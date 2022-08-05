using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recipes.Web;
using Recipes.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("API", x => x.BaseAddress = new Uri(builder.Configuration["API"]));

builder.Services.AddScoped<IIngredientsService, IngredientsService>();

await builder.Build().RunAsync();
