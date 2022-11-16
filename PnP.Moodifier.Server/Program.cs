using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MightyPotato.PnP.Moodifier.Server.Audio;
using MightyPotato.PnP.Moodifier.Server.Audio.Services;
using MightyPotato.PnP.Moodifier.Server.Configuration;
using MightyPotato.PnP.Moodifier.Server.Connection;
using MightyPotato.PnP.Moodifier.Server.SocketConnection.Services;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddJsonFile("appsettings.json")
    .Build();

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration(configurationBuilder =>
{
    configurationBuilder.Sources.Clear();
    configurationBuilder.AddConfiguration(configuration);
});
builder.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
});

builder.ConfigureServices((context, services) =>
{
    var config = context.Configuration;
    services.Configure<HostConfig>(config.GetSection("Hosting"));
    services.Configure<AudioConfig>(config.GetSection("Audio"));
    services.AddSingleton<SocketService>();
    services.AddSingleton<IHostedService>(p => p.GetService<SocketService>()!);
    services.AddSingleton<AudioService>();
    services.AddSingleton<IHostedService>(p => p.GetService<AudioService>()!);
});

builder.UseConsoleLifetime();
var app = builder.Build();
_ = app.RunAsync();

_ = Task.Delay(10);

var audioService = app.Services.GetService<AudioService>();
_ = audioService!.PlayFromPlaylist("Playlist1");
await Task.Delay(5000);
_ = audioService.PlayFromPlaylist("Playlist2");

await Task.Delay(10000);

//var client = new ClientMock("127.0.0.1", 42069);
//await client.Start();