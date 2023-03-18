using Microsoft.AspNetCore.Mvc;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Services.Audio;

namespace MightyPotato.PnP.Moodifier.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly ILogger<PlaylistsController> _logger;
    private readonly PlaylistService _playlistService;

    public PlaylistsController(ILogger<PlaylistsController> logger,PlaylistService playlistService)
    {
        _logger = logger;
        _playlistService = playlistService;
    }
    
    [HttpGet("{*path}")]
    public ActionResult<Playlist> Get(string? path)
    {
        try
        {
            return Ok(_playlistService.GetPlaylistByPath(path));
        }
        catch (NullReferenceException e)
        {
            _logger.LogError("Path {Path} has thrown exception: {Message} {NewLine} {StackTrace}", path, e.Message,
                Environment.NewLine, e.StackTrace);
            return BadRequest("No such playlist");
        }
    }
    [HttpGet()]
    public ActionResult<List<Playlist>> Get()
    {
        return _playlistService.GetAllPlaylists();
    }
}