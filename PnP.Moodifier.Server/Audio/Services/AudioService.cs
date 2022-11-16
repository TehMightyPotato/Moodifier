using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Configuration;
using Newtonsoft.Json;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Services;

public class AudioService : IHostedService
{
    private ILogger<AudioService> _logger;
    private AudioConfig _config;

    private FadingAudioPlaybackContainer _currentPlaybackContainer = null!;
    private FadingAudioPlaybackContainer _temporaryPlaybackContainer = null!;

    private PlaylistsContainer _playlistsContainer;

    public AudioService(ILogger<AudioService> logger, IOptions<AudioConfig> audioConfig)
    {
        _logger = logger;
        _config = audioConfig.Value;
        var json = File.ReadAllText(_config.PlaylistDataPath!);
        _playlistsContainer = JsonConvert.DeserializeObject<PlaylistsContainer>(json);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Audio Service starting");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Audio Service stopping");
        _currentPlaybackContainer.Dispose();
        _temporaryPlaybackContainer.Dispose();
        return Task.CompletedTask;
    }

    public async Task PlayFromPlaylist(string name)
    {
        var playlist = _playlistsContainer.GetByName(name);
        var songPath = playlist.GetRandom();
        if (_currentPlaybackContainer == null)
        {
            _currentPlaybackContainer = new FadingAudioPlaybackContainer(songPath);
            _ = _currentPlaybackContainer.FadeInAsync(3000);
            await _temporaryPlaybackContainer.FadeInAsync(3000);
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

    public Task StopPlaybackAsync()
    {
        _currentPlaybackContainer.Dispose();
        _temporaryPlaybackContainer.Dispose();
        return Task.CompletedTask;
    }
}