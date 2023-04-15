using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MusicPlayer.Client.Shared;

public partial class MainLayout
{
    [Inject]
    public Audio.IAudioPlayer AudioPlayer { get;set; } = null!;
    [Inject]
    public IJSRuntime JSRuntime { get; set; } = null!;

    private bool HasSongLoaded => AudioPlayer.CurrentSong != null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            AudioPlayer.OnLoad += OnSongLoaded;
            await AudioPlayer.Initialize();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnSongLoaded(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}