using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MightyPotato.PnP.Moodifier.Server.Audio.Models;

public sealed class FadingAudioPlaybackContainer: IDisposable
{
    private readonly FadeInOutSampleProvider _fader;
    private readonly WaveOutEvent _waveOut;
    private AudioFileReader _fileReader;

    public event EventHandler? PlaybackStopped;
    
    
    public FadingAudioPlaybackContainer(string filePath)
    {
        _fileReader = new AudioFileReader(filePath);
        _fader = new FadeInOutSampleProvider(_fileReader, true);
        _waveOut = new WaveOutEvent();
        _waveOut.Init(_fader);
        _waveOut.PlaybackStopped += (_, _) => { PlaybackStopped?.Invoke(this, EventArgs.Empty); };
    }

    public async Task FadeInAsync(int durationInMs)
    {
        _fader.BeginFadeIn(durationInMs);
        _waveOut.Play();
        await Task.Delay(durationInMs);
    }

    public async Task FadeOutAsync(int durationInMs)
    {
        _fader.BeginFadeOut(durationInMs);
        await Task.Delay(durationInMs);
        _waveOut.Stop();
    }

    public void Dispose()
    {
        _waveOut.Dispose();
        _fileReader.Dispose();
    }
}