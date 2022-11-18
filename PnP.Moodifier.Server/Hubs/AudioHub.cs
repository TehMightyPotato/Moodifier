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

    public async Task PlayFromPlaylist(string playlistPath)
    {
        try
        {
            await _audioPlaybackService.PlayFromPlaylistAsync(playlistPath);
        }
        catch (NullReferenceException e)
        {
            _logger.LogError("Path {Path} has thrown exception: {Message} {NewLine} {StackTrace}", playlistPath, e.Message,
                Environment.NewLine, e.StackTrace);
        }
    }

    public Task StopPlayback()
    {
        _audioPlaybackService.StopPlayback();
        return Task.CompletedTask;
    }
}