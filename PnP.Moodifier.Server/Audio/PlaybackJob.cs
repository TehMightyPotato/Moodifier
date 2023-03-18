using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MightyPotato.PnP.Moodifier.Server.Audio;

public sealed class PlaybackJob : IDisposable
{
    public event EventHandler? OnTrackFinished;
    public float Volume { get; set; }

    private readonly string _filePath;
    private readonly int _fadeInDuration;
    private readonly int _fadeOutDuration;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public PlaybackJob(string filePath, int fadeInDuration, int fadeOutDuration, float volume)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _filePath = filePath;
        _fadeInDuration = fadeInDuration;
        _fadeOutDuration = fadeOutDuration;
        Volume = volume;
    }

    public void Run()
    {
        Task.Run(() => Run(_cancellationTokenSource.Token));
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task Run(CancellationToken token)
    {
        await using (var fileReader = new AudioFileReader(_filePath))
        using (var waveOut = new WaveOutEvent())
        {
            var fader = new FadeInOutSampleProvider(fileReader, true);
            waveOut.Init(fader);
            waveOut.Volume = Volume;
            waveOut.PlaybackStopped += OnPlaybackStopped;
            fader.BeginFadeIn(_fadeInDuration);
            waveOut.Play();
            try
            {
                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(10, token);
                    if (Math.Abs(waveOut.Volume - Volume) > float.Epsilon)
                    {
                        waveOut.Volume = Volume;
                    }
                    token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                waveOut.PlaybackStopped -= OnPlaybackStopped;
            }

            if (waveOut.PlaybackState == PlaybackState.Playing)
            {
                fader.BeginFadeOut(_fadeOutDuration);
                await Task.Delay(_fadeOutDuration);
            }

            waveOut.PlaybackStopped -= OnPlaybackStopped;
        }
    }

    private void OnPlaybackStopped(object? sender, StoppedEventArgs stoppedEventArgs)
    {
        OnTrackFinished?.Invoke(sender, stoppedEventArgs);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}