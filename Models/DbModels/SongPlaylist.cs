namespace Music_Library_Management_Application.Models.DbModels
{
    public class SongPlaylist
    {
        public int SongId { get; set; }
        public Song Song { get; set; }

        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }
    }

}
