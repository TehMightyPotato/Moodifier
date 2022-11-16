namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public class Playlist
{
    public string? Name { get; set; }
    public List<string>? Songs { get; set; }

    private Queue<string> _songsToPlayQueue = new Queue<string>();

    public string GetRandom()
    {
        if (_songsToPlayQueue.Count == 0)
        {
            Randomize();
        }

        return _songsToPlayQueue?.Dequeue() ?? throw new InvalidOperationException();
    }

    private void Randomize()
    {
        var rand = new Random();
        var tmp = Songs!.OrderBy(i => rand.Next()).ToArray();
        _songsToPlayQueue = new Queue<string>(tmp);
    }
}