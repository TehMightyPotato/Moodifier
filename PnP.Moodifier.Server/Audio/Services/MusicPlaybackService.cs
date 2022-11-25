using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Configuration;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Services;

public sealed class MusicPlaybackService
{
    public event EventHandler<PlaylistElement?>? PlaylistChanged;

    private readonly ILogger<MusicPlaybackService> _logger;
    private readonly PlaylistService _playlistService;
    private readonly AudioConfig _config;

    private PlaybackJob? _currentPlaybackJob;
    private PlaylistElement? _currentPlaylist;

    public MusicPlaybackService(ILogger<MusicPlaybackService> logger, PlaylistService playlistService,
        IOptions<AudioConfig> config)
    {
        _logger = logger;
        _playlistService = playlistService;
        _config = config.Value;
        _logger.LogInformation("AudioPlaybackService loaded");
    }

    public async Task<string?> PlayFromPlaylistAsync(string path)
    {
        var newPlaylist = _playlistService.GetByPath(path);
        return await PlayFromPlaylistAsync(newPlaylist);
    }

    private Task<string> PlayFromPlaylistAsync(PlaylistElement playlistElement)
    {
        _logger.LogInformation("Requested to play: {PlaylistPath}", playlistElement.Path);
        _currentPlaylist = playlistElement;
        _currentPlaybackJob?.Stop();
        _currentPlaybackJob?.Dispose();
        var songPath = playlistElement.GetNext();
        if (_currentPlaybackJob != null) _currentPlaybackJob.OnTrackFinished -= CurrentPlaybackJobOnOnTrackFinished;
        _currentPlaybackJob = new PlaybackJob(songPath, _config.FadeInDuration, _config.FadeOutDuration);
        _currentPlaybackJob.Run();
        _currentPlaybackJob.OnTrackFinished += CurrentPlaybackJobOnOnTrackFinished;
        OnPlaylistChanged(playlistElement);
        return Task.FromResult(playlistElement.Path);
    }

    public void StopPlayback()
    {
        _logger.LogInformation("Stopping Playback");
        _currentPlaybackJob?.Stop();
        _currentPlaybackJob = null;
        _currentPlaylist = null;
        OnPlaylistChanged(null);
    }

    public PlaylistElement? GetCurrentPlaylist()
    {
        return _currentPlaylist ?? null;
    }

    private async void CurrentPlaybackJobOnOnTrackFinished(object? sender, EventArgs e)
    {
        if (_currentPlaylist != null)
        {
            await PlayFromPlaylistAsync(_currentPlaylist);
        }
    }

    private void OnPlaylistChanged(PlaylistElement? e)
    {
        PlaylistChanged?.Invoke(this, e);
    }
}