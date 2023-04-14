namespace MusicPlayer.Client.Audio;

public class Song
{
    public Song(Guid id, string artist, string trackName, string audioSource, int durationSeconds)
    {
        Id = id;
        Artist = artist;
        TrackName = trackName;
        AudioSource = audioSource;
        DurationSeconds = durationSeconds;
    }

    public Guid Id { get; }
    public string Artist { get; }
    public string TrackName { get; }
    public string AudioSource { get; }
    public int DurationSeconds { get; }
}