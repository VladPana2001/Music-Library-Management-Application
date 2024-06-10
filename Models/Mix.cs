namespace Music_Library_Management_Application.Models
{
    public class Mix
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MixSong> Songs { get; set; } = new List<MixSong>();
    }

    public class MixSong
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public double StartTime { get; set; } = 0;
        public double EndTime { get; set; } = 0;
        public int Order { get; set; }
    }
}
