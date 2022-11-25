using MightyPotato.PnP.Moodifier.Server.Audio.Models;

namespace MightyPotato.PnP.Moodifier.Server.Audio;

public class PlaylistLoader
{
    private readonly string[] _musicFileExtensions = new[] { ".mp3", ".wav" };
    private readonly string[] _imageFileExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp"};
    
    private readonly IWebHostEnvironment _webHostEnvironment;

    //Populated by LoadPlaylists
    public List<PlaylistElement> Playlists { get; set; }

    //Populated by LoadPlaylists
    public List<PlaylistElement> PlaylistsStructure { get; set; }

    private string _imageDirectoryPath;

    public PlaylistLoader(string playlistRootPath, IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        Playlists = new List<PlaylistElement>();
        _imageDirectoryPath = webHostEnvironment.WebRootPath + "/images/playlists/";
        CleanupOldImages();
        PlaylistsStructure = LoadPlaylistStructure(playlistRootPath);
    }

    private List<PlaylistElement> LoadPlaylistStructure(string playlistRootPath)
    {
        var playlistStructure = new List<PlaylistElement>();
        var directories = Directory.EnumerateDirectories(playlistRootPath);
        foreach (var subDirectory in directories)
        {
            var element = ParseRecursive(subDirectory, "");
            playlistStructure.Add(element);
            if (element.MusicFiles != null && element.MusicFiles.Count > 0)
            {
                Playlists.Add(element);
            }
        }
        return playlistStructure;
    }
    
    private PlaylistElement ParseRecursive(string fileSystemPath, string playlistPath)
    {
        var thisPlaylistPath = playlistPath + new DirectoryInfo(fileSystemPath).Name;
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
                imagePath = CopyRelatedImage(file, thisPlaylistPath);
            }
        }

        var children = new List<PlaylistElement?>();
        var subDirectories = Directory.EnumerateDirectories(fileSystemPath);
        foreach (var subDirectory in subDirectories)
        {
            var x = ParseRecursive(subDirectory, thisPlaylistPath + "/");
            children.Add(x);

            if (x.MusicFiles?.Count > 0)
            {
                Playlists.Add(x);
            }
        }

        return new PlaylistElement(thisPlaylistPath, imagePath, musicFiles, children);
    }

    private string CopyRelatedImage(string originalImagePath,string? playlistElementPath)
    {
        var newPath = _imageDirectoryPath + playlistElementPath;
        var originalExtension = Path.GetExtension(originalImagePath);
        var fullNewPath = newPath + "/cover" + originalExtension;
        Directory.CreateDirectory(newPath);
        File.Copy(originalImagePath, newPath + "/cover" + originalExtension);
        var a = new Uri(fullNewPath);
        var b = new Uri(_imageDirectoryPath);
        return "/" + b.MakeRelativeUri(a).ToString();
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
            Directory.Delete(subdirectory,true);
        }
    }
}