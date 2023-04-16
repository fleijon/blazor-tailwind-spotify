using Microsoft.AspNetCore.Components;
using MusicPlayer.Client.Audio;

namespace MusicPlayer.Client.Pages;

public partial class Library : IDisposable
{
    [Inject]
    public MusicStore.IMusicStore MusicStore { get;set; } = null!;
    [Inject]
    public IAudioPlayer AudioPlayer { get;set; } = null!;

    [Parameter]
    public Guid AlbumId { get;set; } // = Guid.Parse("95ad48b5-d4fc-4178-8fc9-bd306be760ed");

    private MusicStore.Album? _album;
    private List<MusicStore.Song>? _songs;
    private string _artists = string.Empty;

    private MusicStore.Album? Album => _album ??= (_album = MusicStore.GetAlbum(AlbumId));
    private IReadOnlyList<MusicStore.Song>? Songs => _songs ??= (_songs = Album?.Songs.Select(s => MusicStore.GetSong(s.Id)).Where(s => s is not null).ToList());
    private string Artists => GetArtists();
    private string AlbumName => Album?.Name ?? string.Empty;
    private string AlbumCover => Album?.AlbumCoverSource ?? string.Empty;
    private int? ReleaseYear => Album?.ReleaseYear;
    private bool IsPlaying => AudioPlayer.IsPlaying;

    private string GetArtists()
    {
        if(!string.IsNullOrEmpty(_artists))
            return _artists;
        if(Songs is null)
            return string.Empty;
        var artists = Songs.Select(s => MusicStore.GetArtist(s.ArtistId)).Where(a => !string.IsNullOrEmpty(a?.Name)).ToArray();
        return string.Join(", ", artists!.Select(a => a!.Name).ToArray());
    }

    private async Task QueueAndPlayAlbum()
    {
        if(Songs is null)
            return;

        if(IsPlaying)
        {
            await AudioPlayer.Pause();
        }
        else
        {
            var songsToQueue = Songs.Select(s => new PlaylistItem(s.Id.ToString(), s.Source));

            await AudioPlayer.ClearQueue();
            await AudioPlayer.QueueLast(songsToQueue);
            await AudioPlayer.Play();
        }
    }

    protected override void OnInitialized()
    {
        AudioPlayer.OnPause += PlayChanged;
        AudioPlayer.OnPlay += PlayChanged;
        base.OnInitialized();
    }

    private void PlayChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    void IDisposable.Dispose()
    {
        AudioPlayer.OnPause -= PlayChanged;
        AudioPlayer.OnPlay -= PlayChanged;
    }
}
