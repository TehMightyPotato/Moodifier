using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Configuration;
using Newtonsoft.Json;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Services;

public class AudioPlaybackService : IDisposable
{
    private ILogger<AudioPlaybackService> _logger;
    private readonly PlaylistService _playlistService;

    private FadingAudioPlaybackContainer _currentPlaybackContainer = null!;
    private FadingAudioPlaybackContainer _temporaryPlaybackContainer = null!;


    public AudioPlaybackService(ILogger<AudioPlaybackService> logger, PlaylistService playlistService)
    {
        _logger = logger;
        _playlistService = playlistService;
    }

    public async Task PlayFromPlaylistAsync(string name)
    {
        var playlist = _playlistService.GetByName(name);
        var songPath = playlist.GetRandom();
        if (_currentPlaybackContainer == null)
        {
            _currentPlaybackContainer = new FadingAudioPlaybackContainer(songPath);
            await _currentPlaybackContainer.FadeInAsync(3000);
        }
        else
        {
            _temporaryPlaybackContainer = new FadingAudioPlaybackContainer(songPath);
            _ = _currentPlaybackContainer.FadeOutAsync(3000);
            await _temporaryPlaybackContainer.FadeInAsync(3000);
            _currentPlaybackContainer.Dispose();
            _currentPlaybackContainer = _temporaryPlaybackContainer;
        }
    }

    public void StopPlayback()
    {
        _currentPlaybackContainer.Dispose();
        _temporaryPlaybackContainer.Dispose();
    }

    public void Dispose()
    {
        _currentPlaybackContainer.Dispose();
        _temporaryPlaybackContainer.Dispose();
    }
}