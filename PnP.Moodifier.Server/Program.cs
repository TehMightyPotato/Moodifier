using MightyPotato.PnP.Moodifier.Server.Audio.Services;
using MightyPotato.PnP.Moodifier.Server.Configuration;
using MightyPotato.PnP.Moodifier.Server.Hubs;


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
builder.Services.AddSingleton<MusicPlaybackService>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddEndpointsApiExplorer();

builder.WebHost.UseWebRoot("wwwroot").UseStaticWebAssets();

var app = builder.Build();
app.UseRouting();
app.MapRazorPages();
app.MapHub<AudioHub>("/Audio");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();
app.UseStaticFiles();
await app.RunAsync();