namespace MightyPotato.PnP.Moodifier.Server.Configuration;

public class AudioConfig
{
    public string ConnectionSoundPath { get; set; } = null!;
    public string DisconnectionSoundPath { get; set; } = null!;
    public int PlaybackEndedEventOffsetInMs { get; set; }
    public string PlaylistPath { get; set; } = null!;

    public int FadeInDuration { get; set; }

    public int FadeOutDuration { get; set; }
}