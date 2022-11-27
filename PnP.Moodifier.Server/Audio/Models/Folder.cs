using MightyPotato.PnP.Moodifier.Server.Audio.Interfaces;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public class Folder : IPlaylistElement
{
    public string Path { get; set; }
    
    public string? ImagePath { get; set; }

    public List<IPlaylistElement> Content { get; set; }

    public Folder(string path, string? imagePath, List<IPlaylistElement> content)
    {
        Path = path;
        ImagePath = imagePath;
        Content = content;
    }
}