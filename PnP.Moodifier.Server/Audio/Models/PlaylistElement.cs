namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public class PlaylistElement
{
    public string Path { get; set; }

    public string? ImagePath { get; set; }

    public List<string>? MusicFiles { get; set; }

    public List<PlaylistElement?> Children { get; set; }

    private Queue<string> _songsToPlayQueue = new();

    public PlaylistElement(string path, string? imagePath, List<string>? musicFiles, List<PlaylistElement?> children)
    {
        Path = path;
        ImagePath = imagePath;
        MusicFiles = musicFiles;
        Children = children;
    }

    public string GetNext()
    {
        if (_songsToPlayQueue.Count == 0)
        {
            Randomize();
        }

        return _songsToPlayQueue.Dequeue() ?? throw new InvalidOperationException();
    }

    private void Randomize()
    {
        var rand = new Random();
        var tmp = MusicFiles!.OrderBy(_ => rand.Next()).ToArray();
        _songsToPlayQueue = new Queue<string>(tmp);
    }
}