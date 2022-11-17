using Microsoft.AspNetCore.Mvc;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Audio.Services;

namespace MightyPotato.PnP.Moodifier.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly PlaylistService _playlistService;

    public PlaylistsController(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }
    
    [HttpGet("{name}")]
    public ActionResult<Playlist> Get(string name)
    {
        return _playlistService.GetByName(name);
    }
    [HttpGet()]
    public ActionResult<List<Playlist>> Get()
    {
        return _playlistService.GetAll();
    }
}