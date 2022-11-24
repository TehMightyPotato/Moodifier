using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MightyPotato.PnP.Moodifier.Server.Audio;

public class AudioPlaybackEngine : IDisposable
{
    public event EventHandler? PlaybackStopped;
    private readonly IWavePlayer _outputDevice;
    private readonly MixingSampleProvider _mixer;

    public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
    {
        _outputDevice = new WaveOutEvent();
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount))
        {
            ReadFully = true
        };
        _outputDevice.Init(_mixer);
        _outputDevice.PlaybackStopped += (sender, args) => PlaybackStopped?.Invoke(this, EventArgs.Empty);
        _outputDevice.Play();
    }

    public void AddMixerInput(ISampleProvider input)
    {
        _mixer.AddMixerInput(input);
    }

    public void RemoveMixerInput(ISampleProvider input)
    {
        _mixer.RemoveMixerInput(input);
    }

    public void Dispose()
    {
        _outputDevice.Dispose();
    }
}