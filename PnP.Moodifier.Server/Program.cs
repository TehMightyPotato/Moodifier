using MightyPotato.PnP.Moodifier.Server.Audio.Services;
using MightyPotato.PnP.Moodifier.Server.Configuration;
using MightyPotato.PnP.Moodifier.Server.Hubs;
using Microsoft.AspNetCore.SignalR.Client;


var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(configuration);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSignalR();
builder.Services.Configure<AudioConfig>(configuration.GetSection("Audio"));
builder.Services.AddSingleton<PlaylistService>();
builder.Services.AddSingleton<AudioPlaybackService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseRouting();
app.MapHub<AudioHub>("/Audio");
app.MapControllers();
await app.RunAsync();

return;
var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:42069/Audio")
    .Build();
connection.Closed += async (error) =>
{
    await Task.Delay(new Random().Next(0, 5) * 1000);
    await connection.StartAsync();
};
await connection.StartAsync();
await Task.Delay(100);
await connection.InvokeAsync("PlayFromPlaylist", "Playlist2");
await Task.Delay(5000);
await connection.InvokeAsync("PlayFromPlaylist", "Playlist2");
await Task.Delay(5000);
await connection.InvokeAsync("StopPlayback");
await Task.Delay(5000);
await connection.InvokeAsync("PlayFromPlaylist", "Playlist2");
await Task.Delay(5000);
await connection.InvokeAsync("PlayFromPlaylist", "Playlist2");