using Microsoft.AspNetCore.Components;

namespace MusicPlayer.Client.Audio;

public interface IAudioManager
{
    Song? CurrentSong { get; }
    IAudioPlayer? AudioPlayer { get; }
    event EventHandler? AudioPlayerIsSet;
    event EventHandler? StateChanged;

    Task InitializeAudioPlayer(IAudioPlayer audioPlayer);
    Task LoadSong(Song song);
    void UnLoadSong();

    Task Pause();
    Task Play();
    Task Clear();
    Task Queue(IEnumerable<Song> songs);
    Task Next();
    Task Previous();
}

public class AudioManager : IAudioManager
{
    private async void OnSongEnded(object? sender, EventArgs e)
    {
        await Next();
    }

    private Song? currentSong;

    private readonly List<Song> _records = new();
    private int _currentRecordIndex;
    public IAudioPlayer? AudioPlayer { get;private set; }

    public Song? CurrentSong
    {
        get => currentSong;
    }

    public event EventHandler? AudioPlayerIsSet;
    public event EventHandler? StateChanged;

    private void NotifyStateChanged() => StateChanged?.Invoke(this, EventArgs.Empty);

    public async Task LoadSong(Song song)
    {
        currentSong = song;
        if(AudioPlayer != null)
        {
            await AudioPlayer.Load();
        }

        NotifyStateChanged();
    }

    public void UnLoadSong()
    {
        currentSong = null;

        NotifyStateChanged();
    }

    public async Task Pause()
    {
        if(AudioPlayer is null)
            return;

        await AudioPlayer.PauseAudio();

        NotifyStateChanged();
    }

    public async Task Play()
    {
        if(AudioPlayer is null)
            return;

        if(currentSong == null)
        {
            if(_records.Count == 0)
            {
                return;
            }
            else
            {
                await LoadSong(_records[_currentRecordIndex]);
            }
        }

        await AudioPlayer.PlayAudio();

        NotifyStateChanged();
    }

    public async Task Next()
    {
        if(_currentRecordIndex >= _records.Count - 1)
            return;

        _currentRecordIndex++;

        await LoadSong(_records[_currentRecordIndex]);
        await Play();

        NotifyStateChanged();
    }

    public async Task Previous()
    {
        if(_currentRecordIndex == 0)
            return;

        _currentRecordIndex--;

        await LoadSong(_records[_currentRecordIndex]);
        await Play();

        NotifyStateChanged();
    }

    public async Task Clear()
    {
        await Pause();
        currentSong = null;
        _records.Clear();

        NotifyStateChanged();
    }

    public async Task Queue(IEnumerable<Song> songs)
    {
        _records.AddRange(songs);
        await Play();

        NotifyStateChanged();
    }

    public async Task InitializeAudioPlayer(IAudioPlayer audioPlayer)
    {
        AudioPlayer = audioPlayer;
        AudioPlayer.Volume = 50;

        AudioPlayer.OnSongEnded += OnSongEnded;
        AudioPlayer.OnVolumeChanged += OnVolumeChanged;
        AudioPlayer.IsPlayingChanged += OnIsPlayingChanged;

        await AudioPlayer.InitializeAudio();

        AudioPlayerIsSet?.Invoke(this, EventArgs.Empty);
    }

    private void OnIsPlayingChanged(object? sender, bool e)
    {
        NotifyStateChanged();
    }

    private void OnVolumeChanged(object? sender, double e)
    {
        NotifyStateChanged();
    }
}
