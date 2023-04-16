using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MusicPlayer.Client.Audio;

public interface IAudioPlayer
{
    event EventHandler? OnPlay;
    event EventHandler? OnLoad;
    event EventHandler? OnEnd;
    event EventHandler? OnPause;
    event EventHandler? OnSeek;
    event EventHandler<double>? OnStepChanged;
    event EventHandler<double>? OnVolumeChanged;
    event EventHandler<double>? OnTimeUpdated;

    Task Initialize();
    Task QueueLast(IEnumerable<PlaylistItem> songs);
    Task QueueNext(IEnumerable<PlaylistItem> songs);
    Task ClearQueue();
    Task Play();
    Task Pause();
    Task Seek(double position);
    Task GoNext();
    Task GoPrevious();

    PlaylistItem? CurrentSong { get; }
    double Volume { get; set; }
    double Time { get; set; }
    bool IsPlaying { get; }
}

public class PlaylistItem
{
    public PlaylistItem(string id, string audioSource)
    {
        Id = id;
        AudioSource = audioSource;
    }

    public string Id { get; }
    public string AudioSource { get; }
}

public sealed class AudioWrapper : IAudioPlayer
{
    public AudioWrapper(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    private const string moduleSource = "./js/audio.js";
    private DotNetObjectReference<AudioWrapper>? _ref;
    private IJSObjectReference? _jsModule;
    private double _volume = 50;
    public double Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            SetVolume(_volume);
        }
    }
    private double _time;
    private readonly IJSRuntime _jsRuntime;

    public double Time
    {
        get => _time;
        set
        {
            //_time = value;
            SetTime(_time);
        }
    }

    public bool IsPlaying { get; private set; }
    public bool IsInitialized => _jsModule != null && _ref != null;
    public PlaylistItem? CurrentSong { get;private set; }

    public event EventHandler<double>? OnTimeUpdated;
    public event EventHandler? OnSongEnded;
    public event EventHandler<double>? OnVolumeChanged;
    public event EventHandler? OnPlay;
    public event EventHandler? OnLoad;
    public event EventHandler? OnUnload;
    public event EventHandler? OnPause;
    public event EventHandler? OnSeek;
    public event EventHandler? OnEnd;
    public event EventHandler<double>? OnStepChanged;

    public async Task PlayAudio() => await InvokeAsync("playAudio");

    public async Task PauseAudio() => await InvokeAsync("pauseAudio");

    private async Task SetVolume(double volume) => await InvokeAsync("setVolume", volume > 1 ? volume / 100 : volume);

    private async void SetTime(double time) => await InvokeAsync("setTime", time);

    private async Task InvokeAsync(string method, params object?[]? args)
    {
        if(!IsInitialized)
            throw new InvalidOperationException("Js Module is not intitialized");
        await _jsModule!.InvokeVoidAsync(method, args);
    }

    public async Task QueueLast(IEnumerable<PlaylistItem> songs)
    {
        foreach(var item in songs)
        {
            await InvokeAsync("queueLast", item);
        }
    }

    public Task QueueNext(IEnumerable<PlaylistItem> songs)
    {
        throw new NotImplementedException();
    }

    public async Task ClearQueue() => await InvokeAsync("clearQueue");

    public async Task Play() => await InvokeAsync("play");

    public async Task Stop() => await InvokeAsync("stop");

    public async Task Pause() => await InvokeAsync("pause");

    public Task Seek(double position)
    {
        throw new NotImplementedException();
    }

    public async Task GoNext() => await InvokeAsync("next");

    public async Task GoPrevious() => await InvokeAsync("previous");

    public async Task Initialize()
    {
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", moduleSource);
        _ref = DotNetObjectReference.Create(this);

        await _jsModule.InvokeVoidAsync("initializePlayer", _ref);
        await SetVolume(_volume);
    }

    [JSInvokable]
    public Task OnEndJs()
    {
        OnSongEnded?.Invoke(this, EventArgs.Empty);

        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnTimeChangedJs(double time)
    {
        _time = time;
        OnTimeUpdated?.Invoke(this, time);

        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task OnVolumeChangedJs()
    {
        if(!IsInitialized)
            return;
        var volumeStr = await _jsModule!.InvokeAsync<string>("getVolume");

        if(string.IsNullOrWhiteSpace(volumeStr))
            return;

        if(double.TryParse(volumeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedVolume))
        {
            OnVolumeChanged?.Invoke(this, parsedVolume);
        }
    }

    [JSInvokable]
    public Task OnPlayJs()
    {
        IsPlaying = true;

        OnPlay?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnPauseJs()
    {
        IsPlaying = false;

        OnPause?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task AudioLoadedJs(PlaylistItem playlistItem)
    {
        CurrentSong = playlistItem;
        OnLoad?.Invoke(this, EventArgs.Empty);

        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task AudioUnLoadedJs()
    {
        CurrentSong = null;
        OnUnload?.Invoke(this, EventArgs.Empty);

        return Task.CompletedTask;
    }

}