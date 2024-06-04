using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Models
{
    public class PlaylistDetailsViewModel
    {
        public Playlist Playlist { get; set; }
        public IEnumerable<Song> Songs { get; set; }
    }
}
