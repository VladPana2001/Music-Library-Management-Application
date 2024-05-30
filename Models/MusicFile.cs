namespace Music_Library_Management_Application.Models
{
    public class MusicFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}
