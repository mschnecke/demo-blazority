using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Demo.Blazor.Clarity.Client;
using Demo.Blazor.Clarity.Client.Pages;
using StrawberryShake;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services
	.AddBlazorityClient(ExecutionStrategy.CacheAndNetwork)
	.ConfigureHttpClient(x => x.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}graphql"));

builder.Services.AddTransient<IUserService, UserService>();
await builder.Build().RunAsync();

