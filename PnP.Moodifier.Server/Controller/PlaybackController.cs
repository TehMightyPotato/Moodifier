using Microsoft.AspNetCore.Mvc;
using MightyPotato.PnP.Moodifier.Server.Audio.Models;
using MightyPotato.PnP.Moodifier.Server.Services.Audio;

namespace MightyPotato.PnP.Moodifier.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class PlaybackController : ControllerBase
{
    private MusicPlaybackService _playbackService;
    private readonly ILogger<PlaybackController> _logger;

    public PlaybackController(MusicPlaybackService playbackService, ILogger<PlaybackController> logger)
    {
        _playbackService = playbackService;
        _logger = logger;
    }

    [HttpGet("Play/{*path}")]
    public async Task<ActionResult<Playlist>> Play(string path)
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
            _logger.LogWarning("Exception: {E}", e);
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