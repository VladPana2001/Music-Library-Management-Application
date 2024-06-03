namespace Music_Library_Management_Application.Models.DbModels
{
    public class Playlist
    {
        public int Id { get; set; }
        public string? PlaylistTitle { get; set; }
        public string? PlaylistDescription { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<SongPlaylist> SongPlaylists { get; set; }
    }
}
