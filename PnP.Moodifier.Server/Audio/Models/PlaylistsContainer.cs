namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public class PlaylistsContainer
{
    public List<Playlist>? Playlists { get; set; }

    public Playlist GetByName(string name)
    {
        return Playlists!.FirstOrDefault(x => x.Name == name) ?? throw new NullReferenceException("No Playlist with that name found. Bad data?");
    }
}