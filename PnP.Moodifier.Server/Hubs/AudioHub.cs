using Microsoft.AspNetCore.SignalR;
using MightyPotato.PnP.Moodifier.Server.Services.Audio;

namespace MightyPotato.PnP.Moodifier.Server.Hubs;

public class AudioHub : Hub
{
    private readonly ILogger<AudioHub> _logger;
    private readonly MusicPlaybackService _musicPlaybackService;

    public AudioHub(ILogger<AudioHub> logger, MusicPlaybackService musicPlaybackService)
    {
        _logger = logger;
        _musicPlaybackService = musicPlaybackService;
    }

    public async Task PlayFromPlaylist(string playlistPath)
    {
        try
        {
            await _musicPlaybackService.PlayFromPlaylistAsync(playlistPath);
        }
        catch (NullReferenceException e)
        {
            _logger.LogError("Path {Path} has thrown exception: {Message} {NewLine} {StackTrace}", playlistPath, e.Message,
                Environment.NewLine, e.StackTrace);
        }
    }

    public Task StopPlayback()
    {
        _musicPlaybackService.StopPlayback();
        return Task.CompletedTask;
    }
}