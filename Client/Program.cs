using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MusicPlayer.Client.Audio;
using MusicPlayer.Client.MusicStore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<MusicPlayer.Client.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IAudioPlayer, AudioWrapper>();
builder.Services.AddSingleton<IMusicStore, MusicStore>();

await builder.Build().RunAsync();
