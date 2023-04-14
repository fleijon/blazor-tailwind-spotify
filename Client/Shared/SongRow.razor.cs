using Microsoft.AspNetCore.Components;
using MusicPlayer.Client.Audio;

namespace MusicPlayer.Client.Shared;

public partial class SongRow
{
    [Inject]
    public MusicStore.IMusicStore MusicStore { get;set; } = null!;
    [Inject]
    public Audio.IAudioManager AudioManager { get;set; } = null!;
    [Parameter]
    public Guid? SongId { get;set; }
    [Parameter]
    public int? Index { get;set; }

    private string? Name;
    private string? TrackTimeFormatted;
    private string? Artist;
    private string TextColor => IsLoaded ? "text-white" : "text-gray-400";
    private bool IsLoaded => (SongId is null || AudioManager.CurrentSong is null) ? false : SongId == AudioManager.CurrentSong.Id;
    private bool IsPlaying => IsLoaded && (AudioManager.AudioPlayer != null && AudioManager!.AudioPlayer.IsPlaying);
    private bool isHover = false;

    private async void OnPlayPauseClick()
    {
        if(IsPlaying)
        {
            await AudioManager.Pause();
        }
        else
        {
            if(!SongId.HasValue)
                return;

            var song = MusicStore.GetSong(SongId.Value);

            if(song is null)
                return;

            var artist = MusicStore.GetArtist(song.ArtistId);
            await AudioManager.Clear();
            await AudioManager.Queue(new Song[]{ new Song(SongId.Value, artist?.Name ?? "Unknown artist", song.Name, song.Source, song.Duration)});
        }
    }
    private void OnMouseEnter()
    {
        isHover = true;
    }
    private void OnMouseLeave()
    {
        isHover = false;
    }

    protected override void OnInitialized()
    {
        if(SongId.HasValue)
        {
            var song = MusicStore.GetSong(SongId.Value);
            if(song is not null)
            {
                Name = song.Name;
                TrackTimeFormatted = song.TrackTimeFormatted();
                Artist = MusicStore.GetArtist(song.ArtistId)?.Name ?? string.Empty;
            }
        }

        if(AudioManager.AudioPlayer is null)
        {
            AudioManager.AudioPlayerIsSet += AudioPlayerIsSet;
        }
        else
        {
            AudioManager.AudioPlayer.IsPlayingChanged += OnIsPlayingChanged;
        }

        base.OnInitialized();
    }

    private void AudioPlayerIsSet(object? sender, EventArgs e)
    {
        StateHasChanged();

        if(AudioManager.AudioPlayer is null)
            return;

        AudioManager.AudioPlayer.IsPlayingChanged += OnIsPlayingChanged;
    }

    private void OnIsPlayingChanged(object? sender, bool e)
    {
        StateHasChanged();
    }
}

public static class SongExtensions
{
    public static string TrackTimeFormatted(this MusicStore.Song target) => $"{(int)Math.Floor((double)target.Duration/60)}:{(int)(target.Duration % 60):00}";
}
