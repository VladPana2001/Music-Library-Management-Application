namespace Music_Library_Management_Application.Models.DbModels
{
    public class MixDb
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public byte[] MixFile { get; set; }
        public double LimiterThreshold { get; set; } = 0.9;

        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<MixSongDb> MixSongs { get; set; } = new List<MixSongDb>();

    }
}
