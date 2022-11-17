using Microsoft.AspNetCore.SignalR;
using MightyPotato.PnP.Moodifier.Server.Audio.Services;

namespace MightyPotato.PnP.Moodifier.Server.Hubs;

public class AudioHub : Hub
{
    private readonly ILogger<AudioHub> _logger;
    private readonly AudioPlaybackService _audioPlaybackService;

    public AudioHub(ILogger<AudioHub> logger, AudioPlaybackService audioPlaybackService)
    {
        _logger = logger;
        _audioPlaybackService = audioPlaybackService;
    }

    public async Task PlayFromPlaylist(string playlistName)
    {
        await _audioPlaybackService.PlayFromPlaylistAsync(playlistName);
    }

    public Task StopPlayback()
    {
        _audioPlaybackService.StopPlayback();
        return Task.CompletedTask;
    }
}