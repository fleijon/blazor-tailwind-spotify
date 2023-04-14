using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MusicPlayer.Client.Audio;

public interface IAudioPlayer
{
    event EventHandler? OnSongEnded;
    event EventHandler<double>? OnTimeUpdated;
    event EventHandler<double>? OnVolumeChanged;
    event EventHandler<bool>? IsPlayingChanged;
    Task InitializeAudio();
    Task PlayAudio();
    Task StopAudio();
    Task PauseAudio();
    Task Load();
    double Volume { get; set; }
    double Time { get; set; }
    bool IsPlaying { get; }
}

public sealed class AudioWrapper : IAudioPlayer
{
    private const string moduleSource = "./js/audio.js";
    private readonly DotNetObjectReference<AudioWrapper> _ref;
    private readonly IJSObjectReference _jsModule;
    private readonly ElementReference _elementReference;

    private double _volume;
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

    public event EventHandler<double>? OnTimeUpdated;
    public event EventHandler? OnSongEnded;
    public event EventHandler<double>? OnVolumeChanged;
    public event EventHandler<bool>? IsPlayingChanged;

    private AudioWrapper(IJSObjectReference jsModule, ElementReference audioElementReference)
    {
        _jsModule = jsModule;
        _elementReference = audioElementReference;
        _ref = DotNetObjectReference.Create(this);
    }

    public static async Task<AudioWrapper> Create(IJSRuntime jSRuntime, ElementReference audioElementReference)
    {
        var jsModule = await jSRuntime.InvokeAsync<IJSObjectReference>("import", moduleSource);

        return new AudioWrapper(jsModule, audioElementReference);
    }

    public async Task InitializeAudio()
    {
        await _jsModule.InvokeVoidAsync("initAudio", _elementReference, _ref);
    }

    public async Task Load()
    {
        await _jsModule.InvokeVoidAsync("loadAudio", _elementReference);
    }

    public async Task PlayAudio()
    {
        await _jsModule.InvokeVoidAsync("playAudio", _elementReference);
    }

    public async Task PauseAudio()
    {
        await _jsModule.InvokeVoidAsync("pauseAudio", _elementReference);
    }

    public async Task StopAudio()
    {
        await _jsModule.InvokeVoidAsync("stopAudio", _elementReference);
    }

    private async void SetVolume(double volume)
    {
        await _jsModule.InvokeVoidAsync("setVolume", _elementReference, volume > 1 ? volume / 100 : volume);
    }

    private async void SetTime(double time)
    {
        await _jsModule.InvokeVoidAsync("setTime", _elementReference, time);
    }

    [JSInvokable]
    public Task OnEndJsFunction()
    {
        OnSongEnded?.Invoke(this, EventArgs.Empty);

        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task OnTimeUpdateJsFunction()
    {
        var currentTimeStr = await _jsModule.InvokeAsync<string>("getCurrentTime", _elementReference);

        if(string.IsNullOrWhiteSpace(currentTimeStr))
            return;

        if(double.TryParse(currentTimeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedTime))
        {
            _time = parsedTime;
            OnTimeUpdated?.Invoke(this, parsedTime);
        }
    }

    [JSInvokable]
    public async Task OnVolumeChangeJsFunction()
    {
        var volumeStr = await _jsModule.InvokeAsync<string>("getVolume", _elementReference);

        if(string.IsNullOrWhiteSpace(volumeStr))
            return;

        if(double.TryParse(volumeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedVolume))
        {
            OnVolumeChanged?.Invoke(this, parsedVolume);
        }
    }

    [JSInvokable]
    public Task OnPlayJsFunction()
    {
        IsPlaying = true;

        IsPlayingChanged?.Invoke(this, IsPlaying);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnPauseJsFunction()
    {
        IsPlaying = false;

        IsPlayingChanged?.Invoke(this, IsPlaying);
        return Task.CompletedTask;
    }
}