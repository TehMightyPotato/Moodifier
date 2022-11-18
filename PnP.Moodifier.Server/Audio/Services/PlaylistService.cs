using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Configuration;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Services;

public class PlaylistService
{
    private readonly ILogger<PlaylistService> _logger;
    private readonly AudioConfig _config;

    private PlaylistHandler _playlistHandler;

    public PlaylistService(ILogger<PlaylistService> logger, IOptions<AudioConfig> config)
    {
        _logger = logger;
        _config = config.Value;
        _playlistHandler = new PlaylistHandler(_config.PlaylistPath);
        _logger.LogInformation("Playlist Handler loaded!");
    }

    public List<PlaylistElement> GetAll()
    {
        return _playlistHandler.Playlists;
    }

    public List<PlaylistElement> GetStructure()
    {
        return _playlistHandler.GetStructure();
    }

    public PlaylistElement GetByPath(string name)
    {
        return _playlistHandler.GetByPath(name);
    }

    public void Reload()
    {
        _logger.LogWarning("Reloading playlist handler...");
        _playlistHandler = new PlaylistHandler(_config.PlaylistPath);
        _logger.LogWarning("Playlist Handler loaded!");
    }
}