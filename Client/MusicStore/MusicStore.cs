using System.Linq;

namespace MusicPlayer.Client.MusicStore;

public interface IMusicStore
{
    Album? GetAlbum(Guid id);
    Song? GetSong(Guid id);
    Artist? GetArtist(Guid id);
}

public record Album(Guid Id, string Name, IReadOnlyList<AlbumSong> Songs, string AlbumCoverSource, int ReleaseYear);
public record AlbumSong(Guid Id, int Order);
public record Artist(Guid Id, string Name);
public record Song(Guid Id, string Name, int Duration, string Source, Guid ArtistId);

public class MusicStore : IMusicStore
{
    private readonly List<Song> _songs;
    private readonly List<Artist> _artists;
    private readonly List<Album> _albums;

    public MusicStore()
    {
        var artist1 = new Artist(Guid.Parse("7e5f638c-8be3-476d-8d91-f4286b5def41"), "William_King");
        var artist2 = new Artist(Guid.Parse("23c97c30-55ec-4f7b-8d88-2153589cbf59"), "Music_For_Videos");
        var artist3 = new Artist(Guid.Parse("e28e750d-e0ff-400f-83d3-7dcb1cd99cae"), "SoulProdMusic");
        var artist4 = new Artist(Guid.Parse("bd56fa83-fb9f-41c6-a17d-d4660ee73202"), "ComaStudio");
        var artist5 = new Artist(Guid.Parse("5cfa2ae1-5d9d-4a49-be12-e833e60bb226"), "orangery");

        _artists = new List<Artist>()
        {
            artist1,
            artist2,
            artist3,
            artist4,
            artist5
        };

        var song1 = new Song(Guid.Parse("1ef6a8f7-811a-41b6-8ce2-3e1e13b4f562"), "Ambient Classical Guitar", 109, "/media/ambient-classical-guitar-William_King.mp3", artist1.Id);
        var song2 = new Song(Guid.Parse("5b4bacb5-129e-4fbf-ace1-6d4e0084072f"), "Relaxing", 72, "/media/coniferous-forest-orangery.mp3", artist2.Id);
        var song3 = new Song(Guid.Parse("ae7fee78-c14a-4fa1-846d-5dbf8aebeb65"), "Smoke", 118, "/media/floating-abstract-ComaStudio.mp3", artist3.Id);
        var song4 = new Song(Guid.Parse("6f938fbd-fa0f-4a71-95b9-cbeef3bf1889"), "Floating Abstract", 97, "/media/relaxing-Music_For_Videos.mp3", artist4.Id);
        var song5 = new Song(Guid.Parse("212d3113-f9ca-44d8-910e-0aa19f53eb65"), "Coniferous forest", 128, "/media/smoke-SoulProdMusic.mp3", artist5.Id);

        _songs = new List<Song>()
        {
            song1,
            song2,
            song3,
            song4,
            song5
        };

        _albums = new List<Album>()
        {
            new Album(
                Guid.Parse("95ad48b5-d4fc-4178-8fc9-bd306be760ed"),
                "Mixed Album",
                new List<AlbumSong>()
                {
                    new AlbumSong(song1.Id, 1),
                    new AlbumSong(song2.Id, 2),
                    new AlbumSong(song3.Id, 3),
                    new AlbumSong(song4.Id, 4),
                    new AlbumSong(song5.Id, 5)
                },
                "https://picsum.photos/id/31/300/300",
                2023
                )
        };
    }

    public Album? GetAlbum(Guid id) => 
        _albums.Where(a => a.Id == id)
               .Select(a => new Album(a.Id, a.Name, a.Songs.Select(s => new AlbumSong(s.Id, s.Order)).ToArray(), a.AlbumCoverSource, a.ReleaseYear))
               .FirstOrDefault();

    public Song? GetSong(Guid id) =>
        _songs.Where(s => s.Id == id)
              .Select(s => new Song(s.Id, s.Name, s.Duration, s.Source, s.ArtistId))
              .FirstOrDefault();

    public Artist? GetArtist(Guid id) =>
        _artists.Where(a => a.Id == id)
                .Select(a => new Artist(a.Id, a.Name))
                .FirstOrDefault();
}
