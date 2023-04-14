using Microsoft.AspNetCore.Components;
using MusicPlayer.Client.Audio;

namespace MusicPlayer.Client.Pages;

public partial class Library
{
    [Inject]
    public MusicStore.IMusicStore MusicStore { get;set; } = null!;
    [Inject]
    public IAudioManager AudioManager { get;set; } = null!;

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
    private bool IsPlaying => AudioManager.AudioPlayer?.IsPlaying ?? false;

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
            await AudioManager.Pause();
        }
        else
        {
            var songsToQueue = Songs.Select(s => new Song(s.Id, MusicStore.GetArtist(s.ArtistId)?.Name ?? string.Empty, s.Name, s.Source, s.Duration));

            await AudioManager.Clear();
            await AudioManager.Queue(songsToQueue);
        }
    }

    protected override void OnInitialized()
    {
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
