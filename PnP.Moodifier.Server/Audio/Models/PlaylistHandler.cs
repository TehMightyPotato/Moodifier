namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public class PlaylistHandler
{
    //Populated by LoadPlaylists
    public List<PlaylistElement> Playlists { get; set; }

    //Populated by LoadPlaylists
    private List<PlaylistElement> PlaylistsStructure { get; set; }

    private readonly string[] _musicFileExtensions = new[] { ".mp3", ".wav" };
    private readonly string[] _imageFileExtensions = new[] { ".png", ".jpg" };

    public PlaylistHandler(string playlistRootPath)
    {
        Playlists = new List<PlaylistElement>();
        PlaylistsStructure = LoadPlaylists(playlistRootPath);
    }


    public PlaylistElement GetByPath(string path)
    {
        return Playlists.FirstOrDefault(x => x.Path == path) ?? throw new NullReferenceException("Bad playlist path");
    }

    public List<PlaylistElement> GetStructure()
    {
        return PlaylistsStructure;
    }

    private List<PlaylistElement> LoadPlaylists(string playlistRootPath)
    {
        var list = new List<PlaylistElement>();
        var directories = Directory.EnumerateDirectories(playlistRootPath);
        foreach (var subDirectory in directories)
        {
            var x = Parse(subDirectory, "");
            list.Add(x);
            if (x.MusicFiles != null && x.MusicFiles.Count > 0)
            {
                Playlists.Add(x);
            }
        }

        return list;
    }


    private PlaylistElement Parse(string fileSystemPath, string playlistPath)
    {
        var thisPlaylistPath = playlistPath + new DirectoryInfo(fileSystemPath).Name;
        var files = Directory.EnumerateFiles(fileSystemPath, "*.*", SearchOption.TopDirectoryOnly);
        var musicFiles = new List<string>();
        string? imagePath = null;
        string name = "";
        foreach (var file in files)
        {
            if (_musicFileExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
            {
                musicFiles.Add(file);
                continue;
            }

            if (Path.GetFileNameWithoutExtension(file) == "cover")
            {
                imagePath = file;
                continue;
            }

            if (Path.GetFileName(file) == "_name.txt")
            {
                name = File.ReadAllText(file);
            }
            
        }
        var children = new List<PlaylistElement?>();
        var subDirectories = Directory.EnumerateDirectories(fileSystemPath);
        foreach (var subDirectory in subDirectories)
        {
            var x = Parse(subDirectory, thisPlaylistPath + "/");
            children.Add(x);

            if (x.MusicFiles?.Count > 0)
            {
                Playlists.Add(x);
            }
        }

        return new PlaylistElement(thisPlaylistPath, name, imagePath, musicFiles, children);
    }
}