using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Configuration;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Services;

public class PlaylistService
{
    private readonly ILogger<PlaylistService> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly AudioConfig _config;
    
    private List<PlaylistElement> _playlists;

    private List<PlaylistElement> _playlistStructure;

    public PlaylistService(ILogger<PlaylistService> logger, IOptions<AudioConfig> config, IWebHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _config = config.Value;
        var playlistLoader = new PlaylistLoader(_config.PlaylistPath, _hostEnvironment);
        _playlists = playlistLoader.Playlists;
        _playlistStructure = playlistLoader.PlaylistsStructure;
        _logger.LogInformation("Playlists loaded!");
    }

    public List<PlaylistElement> GetAll()
    {
        return _playlists;
    }

    public List<PlaylistElement> GetStructure()
    {
        return _playlistStructure;
    }

    public PlaylistElement GetByPath(string path)
    {
        return _playlists.FirstOrDefault(x => x.Path == path) ?? throw new NullReferenceException("Bad playlist path");
    }

    public void Reload()
    {
        _logger.LogWarning("Reloading playlist handler...");
        var playlistLoader = new PlaylistLoader(_config.PlaylistPath, _hostEnvironment);
        _playlists = playlistLoader.Playlists;
        _playlistStructure = playlistLoader.PlaylistsStructure;
        _logger.LogWarning("Playlist Handler loaded!");
    }
}