using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Audio;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Audio.Services;
using MightyPotato.PnP.Moodifier.Server.Configuration;

namespace MightyPotato.PnP.Moodifier.Server.Services.Audio;

public sealed class MusicPlaybackService
{
    public event EventHandler<Playlist?>? PlaylistChanged;
    public event EventHandler<float>? VolumeChanged;

    public float Volume { get; private set; }

    private readonly ILogger<MusicPlaybackService> _logger;
    private readonly PlaylistService _playlistService;
    private readonly AudioConfig _config;

    private PlaybackJob? _currentPlaybackJob;
    private Playlist? _currentPlaylist;

    public MusicPlaybackService(ILogger<MusicPlaybackService> logger, PlaylistService playlistService,
        IOptions<AudioConfig> config)
    {
        Volume = 1;
        _logger = logger;
        _playlistService = playlistService;
        _config = config.Value;
        _logger.LogInformation("AudioPlaybackService loaded");
    }

    public async Task<string?> PlayFromPlaylistAsync(string path)
    {
        var newPlaylist = _playlistService.GetPlaylistByPath(path);
        return await PlayFromPlaylistAsync(newPlaylist);
    }

    private Task<string> PlayFromPlaylistAsync(Playlist playlist)
    {
        _logger.LogInformation("Requested to play: {PlaylistPath}", playlist.Path);
        _currentPlaylist = playlist;
        _currentPlaybackJob?.Stop();
        _currentPlaybackJob?.Dispose();
        var songPath = playlist.GetNext();
        if (_currentPlaybackJob != null) _currentPlaybackJob.OnTrackFinished -= CurrentPlaybackJobOnTrackFinished;
        _currentPlaybackJob = new PlaybackJob(songPath, _config.FadeInDuration, _config.FadeOutDuration, Volume);
        _currentPlaybackJob.Run();
        _currentPlaybackJob.OnTrackFinished += CurrentPlaybackJobOnTrackFinished;
        OnPlaylistChanged(playlist);
        return Task.FromResult(playlist.Path);
    }

    public void StopPlayback()
    {
        _logger.LogInformation("Stopping Playback");
        _currentPlaybackJob?.Stop();
        _currentPlaybackJob = null;
        _currentPlaylist = null;
        OnPlaylistChanged(null);
    }

    public Playlist? GetCurrentPlaylist()
    {
        return _currentPlaylist ?? null;
    }

    public void SetVolume(float newVolume)
    {
        Volume = newVolume;
        if (_currentPlaybackJob != null) _currentPlaybackJob.Volume = Volume;
    }
    
    private async void CurrentPlaybackJobOnTrackFinished(object? sender, EventArgs e)
    {
        if (_currentPlaylist != null)
        {
            await PlayFromPlaylistAsync(_currentPlaylist);
        }
    }

    private void OnPlaylistChanged(Playlist? e)
    {
        PlaylistChanged?.Invoke(this, e);
    }
}