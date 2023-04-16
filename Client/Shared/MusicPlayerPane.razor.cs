using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MusicPlayer.Client.Audio;
using MusicPlayer.Client.MusicStore;

namespace MusicPlayer.Client.Shared;

public partial class MusicPlayerPane : IDisposable
{
    [Inject]
    public IAudioPlayer AudioPlayer { get;set; } = null!;
    [Inject]
    public IMusicStore MusicStore { get;set; } = null!;

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
    public string CurrentTrackName => AudioPlayer.CurrentSong is null ? string.Empty : GetTrackName(Guid.Parse(AudioPlayer.CurrentSong.Id));
    public string CurrentArtistName => AudioPlayer.CurrentSong is null ? string.Empty : GetArtistName(Guid.Parse(AudioPlayer.CurrentSong.Id));
    private bool IsPlaying => AudioPlayer.IsPlaying;
    private int Duration => AudioPlayer.CurrentSong is null ? 0 : GetDuration(Guid.Parse(AudioPlayer.CurrentSong.Id));
    private double SongTime => AudioPlayer.CurrentSong is null ? 0 : AudioPlayer.Time;
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

    private string GetTrackName(Guid songId)
    {
        var track = MusicStore.GetSong(songId);

        return track?.Name ?? string.Empty;
    }

    private int GetDuration(Guid songId)
    {
        var track = MusicStore.GetSong(songId);

        return (int)(track?.Duration.TotalSeconds ?? 0);
    }

    private string GetArtistName(Guid songId)
    {
        var track = MusicStore.GetSong(songId);
        if(track is null)
            return string.Empty;
        var artist = MusicStore.GetArtist(track.ArtistId);

        return artist?.Name ?? string.Empty;
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
        if(!AudioPlayer.IsPlaying)
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
        await AudioPlayer.GoNext();
    }

    public async Task Previous()
    {
        await AudioPlayer.GoPrevious();
    }

    public async Task PlayAudio()
    {
        await AudioPlayer.Play();
    }

    public async Task PauseAudio()
    {
        await AudioPlayer.Pause();
    }

    protected override void OnInitialized()
    {
        AudioPlayer.OnPlay += OnPlayChanged;
        AudioPlayer.OnPause += OnPlayChanged;
        AudioPlayer.OnTimeUpdated += OnTimeUpdated;

        base.OnInitialized();
    }

    private void OnTimeUpdated(object? sender, double e)
    {
        StateHasChanged();
    }

    private void OnPlayChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        AudioPlayer.OnPlay -= OnPlayChanged;
        AudioPlayer.OnPause -= OnPlayChanged;
        AudioPlayer.OnTimeUpdated -= OnTimeUpdated;
    }
}