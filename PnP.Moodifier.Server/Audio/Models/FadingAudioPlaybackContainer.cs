using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public sealed class FadingAudioPlaybackContainer: IDisposable
{
    private readonly AudioPlaybackEngine _engine;
    private readonly FadeInOutSampleProvider _fader;
    private readonly AudioFileReader _fileReader;

    public event EventHandler? PlaybackStopped;
    
    
    public FadingAudioPlaybackContainer(string filePath, AudioPlaybackEngine engine)
    {
        _engine = engine;
        _fileReader = new AudioFileReader(filePath);
        _fader = new FadeInOutSampleProvider(_fileReader, true);
        _engine.PlaybackStopped += (sender, args) => PlaybackStopped?.Invoke(sender, EventArgs.Empty);
    }

    public async Task FadeInAsync(int durationInMs)
    {
        _fader.BeginFadeIn(durationInMs);
        _engine.AddMixerInput(_fader);
        await Task.Delay(durationInMs);
    }

    public async Task FadeOutAsync(int durationInMs)
    {
        _fader.BeginFadeOut(durationInMs);
        await Task.Delay(durationInMs);
        _engine.RemoveMixerInput(_fader);
    }

    public void Dispose()
    {
        _fileReader.Dispose();
    }
}