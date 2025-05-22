using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NorModifierWeb;
using MudBlazor.Services;
using NorModifierLib.Interfaces;
using NorModifierLib.Services;
using NorModifierWeb.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddScoped<WebSerialPort>();
builder.Services.AddScoped<INorService, NorService>();
builder.Services.AddScoped<IUartService, UartService>();

await builder.Build().RunAsync();
