using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Models
{
    public class MixCreateViewModel
    {
        public string MixName { get; set; }
        public string MixDescription { get; set; }
        public List<Song> AllSongs { get; set; }
        public List<MixSong> MixSongs { get; set; } = new List<MixSong>();
        public double LimiterThreshold { get; set; } = 0.9;

        public double ConvertToSeconds(string length)
        {
            var parts = length.Split(':');
            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int minutes) && int.TryParse(parts[1], out int seconds))
                {
                    return minutes * 60 + seconds;
                }
            }
            return 0;
        }
    }
}
