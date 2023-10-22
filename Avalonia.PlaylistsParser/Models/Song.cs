namespace Avalonia.PlaylistsParser.Models
{
    public class Song
    {
        public string SongName { get; }
        public string ArtistName { get; }
        public string AlbumName { get; }
        public string Duration { get; }

        public Song(string name, string artistName, string albumName, string duration)
        {
            SongName = name;
            ArtistName = artistName;
            AlbumName = albumName;
            Duration = duration;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is Song other)
            {
                return SongName == other.SongName && ArtistName == other.ArtistName && 
                       AlbumName == other.AlbumName && Duration == other.Duration;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (SongName?.GetHashCode() ?? 0) ^ (ArtistName?.GetHashCode() ?? 0) ^ 
                   (AlbumName?.GetHashCode() ?? 0) ^ Duration.GetHashCode();
        }
    }
}
