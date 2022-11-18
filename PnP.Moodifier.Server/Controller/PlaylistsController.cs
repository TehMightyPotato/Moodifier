using Microsoft.AspNetCore.Mvc;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Audio.Services;

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
    public ActionResult<PlaylistElement> Get(string path)
    {
        try
        {
            return Ok(_playlistService.GetByPath(path));
        }
        catch (NullReferenceException e)
        {
            _logger.LogError("Path {Path} has thrown exception: {Message} {NewLine} {StackTrace}", path, e.Message,
                Environment.NewLine, e.StackTrace);
            return BadRequest("No such playlist");
        }
    }
    [HttpGet()]
    public ActionResult<List<PlaylistElement>> Get()
    {
        return _playlistService.GetAll();
    }
}