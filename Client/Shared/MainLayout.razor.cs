using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MusicPlayer.Client.Shared;

public partial class MainLayout
{
    [Inject]
    public Audio.IAudioManager AudioManager { get;set; } = null!;
    [Inject]
    public IJSRuntime JSRuntime { get; set; } = null!;

    private bool HasSongLoaded => AudioManager.CurrentSong != null;
    private string CurrentSongSource => AudioManager.CurrentSong?.AudioSource ?? string.Empty;

    protected override void OnInitialized()
    {
        AudioManager.StateChanged += OnAudioStateChanged;
        base.OnInitialized();
    }

    private ElementReference? AudioCompReference { get; set; }

    private void OnAudioStateChanged(object? sender, EventArgs args)
    {
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender && AudioCompReference.HasValue)
        {
            var audioPlayer = await Audio.AudioWrapper.Create(JSRuntime, AudioCompReference.Value);
            AudioManager.StateChanged += OnStateChanged;
            AudioManager.AudioPlayerIsSet += OnAudioPlayerIsSet;

            await AudioManager.InitializeAudioPlayer(audioPlayer);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnAudioPlayerIsSet(object? sender, EventArgs args)
    {
        if(AudioManager.AudioPlayer is null)
            return;

        AudioManager.AudioPlayer.OnTimeUpdated += OnTimeUpdated;
        AudioManager.AudioPlayer.IsPlayingChanged += OnPlayingStateChanged;
        StateHasChanged();
    }

    private void OnPlayingStateChanged(object? sender, bool e)
    {
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