namespace Music_Library_Management_Application.Models.DbModels
{
    public class MixSongDb
    {
        public int Id { get; set; }
        public int MixDbId { get; set; }
        public MixDb MixDb { get; set; }

        public string Title { get; set; }
        public double StartTime { get; set; } = 0;
        public double EndTime { get; set; } = 0;
        public double StartPosition { get; set; }
        public double FadeInDuration { get; set; }
        public double FadeOutDuration { get; set; }
        public double Volume { get; set; } = 1.0;
    }
}
