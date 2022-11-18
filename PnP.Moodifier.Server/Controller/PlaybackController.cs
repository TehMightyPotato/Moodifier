using Microsoft.AspNetCore.Mvc;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Audio.Services;

namespace MightyPotato.PnP.Moodifier.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class PlaybackController : ControllerBase
{
    private AudioPlaybackService _playbackService;
    private readonly ILogger<PlaybackController> _logger;

    public PlaybackController(AudioPlaybackService playbackService, ILogger<PlaybackController> logger)
    {
        _playbackService = playbackService;
        _logger = logger;
    }

    [HttpGet("Play/{*path}")]
    public async Task<ActionResult<PlaylistElement>> Play(string path)
    {
        try
        {
            return Ok(await _playbackService.PlayFromPlaylistAsync(path));
        }
        catch (NullReferenceException e)
        {
            _logger.LogError("Path {Path} has thrown exception: {Message} {NewLine} {StackTrace}", path, e.Message,
                Environment.NewLine, e.StackTrace);
            return BadRequest("No such playlist");
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning("Tried to fade while fade was still in progress");
            return Conflict("Another fade is in progress");
        }
    }

    [HttpGet("Stop")]
    public ActionResult Stop()
    {
        _playbackService.StopPlayback();
        return Ok("Stopped");
    }
}