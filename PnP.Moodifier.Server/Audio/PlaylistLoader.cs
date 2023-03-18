using MightyPotato.PnP.Moodifier.Server.Audio.Interfaces;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;

namespace MightyPotato.PnP.Moodifier.Server.Audio;

public class PlaylistLoader
{
    private readonly string[] _musicFileExtensions = new[] { ".mp3", ".wav" };
    private readonly string[] _imageFileExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
    //Populated by LoadPlaylists
    public List<Playlist> Playlists { get; set; }

    //Populated by LoadPlaylists
    public Folder PlaylistsStructure { get; set; }

    private string _imageDirectoryPath;

    public PlaylistLoader(string playlistRootPath, IWebHostEnvironment webHostEnvironment)
    {
        Playlists = new List<Playlist>();
        _imageDirectoryPath = webHostEnvironment.WebRootPath + "/images/playlists/";
        CleanupOldImages();
        PlaylistsStructure = LoadPlaylistStructure(playlistRootPath);
    }

    private Folder LoadPlaylistStructure(string playlistRootPath)
    {
        var playlistStructure = new List<IPlaylistElement>();
        var directories = Directory.EnumerateDirectories(playlistRootPath);
        foreach (var subDirectory in directories)
        {
            var element = ParseRecursive(subDirectory, "");
            playlistStructure.Add(element);
            if (element is Playlist playlist)
            {
                Playlists.Add(playlist);
            }
        }

        return new Folder("", null, playlistStructure);
    }

    private IPlaylistElement ParseRecursive(string fileSystemPath, string prevPath)
    {
        var elementPath = prevPath + new DirectoryInfo(fileSystemPath).Name;
        var files = Directory.EnumerateFiles(fileSystemPath, "*.*", SearchOption.TopDirectoryOnly);
        var musicFiles = new List<string>();
        string? imagePath = null;
        foreach (var file in files)
        {
            if (_musicFileExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
            {
                musicFiles.Add(file);
                continue;
            }

            if (Path.GetFileNameWithoutExtension(file) == "_cover" &&
                _imageFileExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
            {
                imagePath = CopyRelatedImage(file, elementPath);
            }
        }

        var children = new List<IPlaylistElement>();
        var subDirectories = Directory.EnumerateDirectories(fileSystemPath);
        foreach (var subDirectory in subDirectories)
        {
            var x = ParseRecursive(subDirectory, elementPath + "/");
            children.Add(x);

            if (x is Playlist playlist)
            {
                Playlists.Add(playlist);
            }
        }

        if (musicFiles.Count > 0)
        {
            return new Playlist(elementPath, imagePath, musicFiles);
        }

        return new Folder(elementPath, imagePath, children);
    }

    private string CopyRelatedImage(string originalImagePath, string? playlistElementPath)
    {
        var newPath = _imageDirectoryPath + playlistElementPath;
        var originalExtension = Path.GetExtension(originalImagePath);
        var fullNewPath = newPath + "/cover" + originalExtension;
        Directory.CreateDirectory(newPath);
        File.Copy(originalImagePath, newPath + "/cover" + originalExtension);
        var a = new Uri(fullNewPath);
        var b = new Uri(_imageDirectoryPath);
        return "/" + b.MakeRelativeUri(a);
    }

    private void CleanupOldImages()
    {
        var files = Directory.EnumerateFiles(_imageDirectoryPath);
        var subdirectories = Directory.EnumerateDirectories(_imageDirectoryPath);
        foreach (var file in files)
        {
            File.Delete(file);
        }

        foreach (var subdirectory in subdirectories)
        {
            Directory.Delete(subdirectory, true);
        }
    }
}