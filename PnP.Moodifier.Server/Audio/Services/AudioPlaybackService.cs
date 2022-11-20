using MightyPotato.PnP.Moodifier.Server.Audio.Enums;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Services;

public class AudioPlaybackService : IDisposable
{
    public event EventHandler<PlaylistElement> PlaylistChanged; 

    private PlaybackState _currentPlaybackState;
    private readonly ILogger<AudioPlaybackService> _logger;
    private readonly PlaylistService _playlistService;
    
    private FadingAudioPlaybackContainer? _currentPlaybackContainer;

    private PlaylistElement? _currentPlaylist;

    public AudioPlaybackService(ILogger<AudioPlaybackService> logger, PlaylistService playlistService)
    {
        _logger = logger;
        _playlistService = playlistService;
    }

    public async Task<string> PlayFromPlaylistAsync(string path)
    {
        var newPlaylist = _playlistService.GetByPath(path);
        if (newPlaylist != _currentPlaylist)
        {
            OnPlaylistChanged(this, newPlaylist);
        }
        _currentPlaylist = newPlaylist;
        return await PlayFromPlaylistAsync(_currentPlaylist!);
    }

    private async Task<string> PlayFromPlaylistAsync(PlaylistElement playlistElement)
    {
        //TODO: Either block this or enforce blocking of input while fade is in progress...
        if (_currentPlaybackState == PlaybackState.Fading) return null;
        var songPath = playlistElement.GetNext();
        _logger.LogInformation("Playing song: {SongPath}", songPath);
        _currentPlaybackState = PlaybackState.Fading;
        if (_currentPlaybackContainer != null)
        {
            //Need to unsubscribe the event handler, otherwise infinite loop with playback -> stop -> PlayNextSongAsync is possible
            _currentPlaybackContainer.PlaybackStopped -= PlayNextSongAsync;
            await _currentPlaybackContainer.FadeOutAsync(3000);
            _currentPlaybackContainer.Dispose();
        }
        _currentPlaybackContainer = new FadingAudioPlaybackContainer(songPath);
        _currentPlaybackContainer.PlaybackStopped += PlayNextSongAsync;
        await _currentPlaybackContainer.FadeInAsync(3000);
        _currentPlaybackState = PlaybackState.Playing;
        return playlistElement.Path;
    }
    
    //Plays new song after current song stopped playing;
    private async void PlayNextSongAsync(object? sender, EventArgs e)
    {
        if (_currentPlaybackState != PlaybackState.Playing) return;
        try
        {
            await PlayFromPlaylistAsync(_currentPlaylist ??
                                        throw new NullReferenceException(
                                            "Attempted to play from playlist which was null"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogInformation("Song ended while fade was in progress");
        }
    }

    public void StopPlayback()
    {
        _currentPlaybackState = PlaybackState.Stopped;
        _currentPlaybackContainer?.Dispose();
    }

    public PlaylistElement? GetCurrentPlaylist()
    {
        return _currentPlaylist;
    }

    private void OnPlaylistChanged(object sender, PlaylistElement newPlaylist)
    {
        PlaylistChanged?.Invoke(this, newPlaylist);
    }

    public void Dispose()
    {
        _currentPlaybackState = PlaybackState.Stopped;
        _currentPlaybackContainer?.Dispose();
    }
}