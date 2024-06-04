using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Models
{
    public class PlaylistCreateViewModel
    {
        public Playlist Playlist { get; set; }
        public List<Song> AllUserSongs { get; set; }
        public List<int> SelectedSongIds { get; set; } = new List<int>();
    }
}
