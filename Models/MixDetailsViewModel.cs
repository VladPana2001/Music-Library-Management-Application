using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Models
{
    public class MixDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double LimiterThreshold { get; set; }
        public List<MixSongDb> Songs { get; set; }
    }
}
