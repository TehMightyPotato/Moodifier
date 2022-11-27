using MightyPotato.PnP.Moodifier.Server.Audio.Interfaces;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public class Playlist : IPlaylistElement
{
    public string Path { get; set; }
    public string? ImagePath { get; set; }
    
    public List<string>? MusicFiles { get; set; }
    
    private Queue<string> _songsToPlayQueue = new();
    
    public Playlist(string path, string? imagePath, List<string>? musicFiles)
    {
        Path = path;
        ImagePath = imagePath;
        MusicFiles = musicFiles;
    }
    
    public string GetNext()
    {
        if (_songsToPlayQueue.Count == 0)
        {
            Randomize();
        }

        return _songsToPlayQueue.Dequeue();
    }

    private void Randomize()
    {
        var rand = new Random();
        var tmp = MusicFiles!.OrderBy(_ => rand.Next()).ToArray();
        _songsToPlayQueue = new Queue<string>(tmp);
    }
}