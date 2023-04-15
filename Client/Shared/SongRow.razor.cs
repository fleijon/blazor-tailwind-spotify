using Microsoft.AspNetCore.Components;
using MusicPlayer.Client.Audio;

namespace MusicPlayer.Client.Shared;

public partial class SongRow
{
    [Inject]
    public MusicStore.IMusicStore MusicStore { get;set; } = null!;
    [Inject]
    public IAudioPlayer AudioPlayer { get;set; } = null!;
    [Parameter]
    public Guid? SongId { get;set; }
    [Parameter]
    public int? Index { get;set; }

    private string? Name;
    private string? TrackTimeFormatted;
    private string? Artist;
    private string TextColor => IsLoaded ? "text-white" : "text-gray-400";
    private bool IsLoaded => (SongId is null || AudioPlayer.CurrentSong is null) ? false : SongId == Guid.Parse(AudioPlayer.CurrentSong.Id);
    private bool IsPlaying => IsLoaded && AudioPlayer.IsPlaying;
    private bool isHover = false;

    private async void OnPlayPauseClick()
    {
        if(IsPlaying)
        {
            await AudioPlayer.Pause();
        }
        else
        {
            if(!SongId.HasValue)
                return;

            var song = MusicStore.GetSong(SongId.Value);

            if(song is null)
                return;

            await AudioPlayer.ClearQueue();
            await AudioPlayer.QueueLast(new PlaylistItem[]{ new PlaylistItem(SongId.Value.ToString(), song.Source)});
            await AudioPlayer.Play();
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

        AudioPlayer.OnLoad += OnSongLoaded;
        AudioPlayer.OnPlay += PlayChanged;
        AudioPlayer.OnPause += PlayChanged;

        base.OnInitialized();
    }

    private void PlayChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void OnSongLoaded(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}

public static class SongExtensions
{
    public static string TrackTimeFormatted(this MusicStore.Song target) => $"{(int)Math.Floor((double)target.Duration.TotalSeconds/60)}:{(int)(target.Duration.TotalSeconds % 60):00}";
}
