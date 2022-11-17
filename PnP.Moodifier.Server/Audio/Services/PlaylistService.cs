using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Configuration;
using Newtonsoft.Json;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Services;

public class PlaylistService
{
    private readonly IOptions<AudioConfig> _config;

    private PlaylistsContainer _playlistsContainer;

    public PlaylistService(IOptions<AudioConfig> config)
    {
        _config = config;
        var json = File.ReadAllText(config.Value.PlaylistDataPath!);
        _playlistsContainer = JsonConvert.DeserializeObject<PlaylistsContainer>(json)!;
    }

    public List<Playlist> GetAll()
    {
        return _playlistsContainer.Playlists!;
    }

    public Playlist GetByName(string name)
    {
        return _playlistsContainer.GetByName(name);
    }
}