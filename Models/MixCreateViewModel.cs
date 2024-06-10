using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Models
{
    public class MixCreateViewModel
    {
        public string MixName { get; set; }
        public string MixDescription { get; set; }
        public List<Song> AllSongs { get; set; }
        public List<MixSong> MixSongs { get; set; } = new List<MixSong>();
    }
}
