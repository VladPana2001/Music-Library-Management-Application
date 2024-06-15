using System.ComponentModel.DataAnnotations;

namespace Music_Library_Management_Application.Models
{
    public class SongViewModel
    {
        public string FileName { get; set; }

        [Required(ErrorMessage = "The Song Title is required.")]
        public string SongTitle { get; set; }
        public string Description { get; set; }
        public string SongArtist { get; set; }
        public string SongAlbum { get; set; }
        public string SongLenght { get; set; }
        public short? SongBPM { get; set; }
    }
}
