using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MusicPlayer.Client.Audio;

namespace MusicPlayer.Client.Shared;

public partial class MusicPlayerPane
{
    [Inject]
    public IAudioManager AudioManager { get;set; } = null!;

    [Parameter]
    public string? Source { get;set; }

    private void OnMouseEnter()
    {
        isHover = true;
    }
    private void OnMouseLeave()
    {
        isHover = false;
    }
    private bool isHover = false;
    private string DotAppearance => isHover ? "" : "rangeDotHidden";
    private string background => isHover ? "bg-green-500" : "bg-white";
    public string CurrentTrackName => AudioManager.CurrentSong?.Artist ?? string.Empty;
    public string CurrentArtistName => AudioManager.CurrentSong?.TrackName ?? string.Empty;
    private bool IsPlaying => AudioManager.AudioPlayer?.IsPlaying ?? false;
    private double Duration => (AudioManager.CurrentSong?.DurationSeconds) ?? 0;
    private double SongTime => AudioManager.CurrentSong is null || AudioManager.AudioPlayer is null ? 0 : AudioManager.AudioPlayer.Time;
    private string DurationFormatted => FormatDuration(Duration);
    private string SongTimeFormatted => FormatDuration(SongTime);
    private double SongProgress => CalculateSongProgress(Duration, SongTime);
    private double SongProgressProxy
    {
        get => SongProgress;
        set
        {
            // do nothing
        }
    }

    private static double CalculateSongProgress(double totalTime, double currentTime)
    {
        var progress = currentTime/totalTime;
        return progress > 1 ? 100 : progress * 100;
    }

    private static string FormatDuration(double durationSeconds)
    {
        var minutes = (int)Math.Floor((double)(durationSeconds / 60));
        var seconds = (int)durationSeconds % 60;

        return $"{minutes}:{seconds:00}";
    }

    private async Task OnPlayPauseClick()
    {
        if(AudioManager.AudioPlayer is null)
            return;

        if(!AudioManager.AudioPlayer.IsPlaying)
        {
            await PlayAudio();
        }
        else
        {
            await PauseAudio();
        }
    }

    public async Task Next()
    {
        await AudioManager.Next();
    }

    public async Task Previous()
    {
        await AudioManager.Previous();
    }

    public async Task PlayAudio()
    {
        await AudioManager.Play();
    }

    public async Task PauseAudio()
    {
        await AudioManager.Pause();
    }

    protected override void OnInitialized()
    {
        AudioManager.StateChanged += OnStateChanged;

        if(AudioManager.AudioPlayer is null)
        {
            AudioManager.AudioPlayerIsSet += OnAudioPlayerIsSet;
        }
        else
        {
            AudioManager.AudioPlayer.OnTimeUpdated += OnTimeUpdated;
        }

        base.OnInitialized();
    }

    private void OnAudioPlayerIsSet(object? sender, EventArgs args)
    {
        if(AudioManager.AudioPlayer is null)
            return;

        AudioManager.AudioPlayer.OnTimeUpdated += OnTimeUpdated;
        StateHasChanged();
    }

    private void OnTimeUpdated(object? sender, double args)
    {
        StateHasChanged();
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}