using Microsoft.AspNetCore.Identity;

namespace Music_Library_Management_Application.Models.DbModels
{
    public class User : IdentityUser
    {
        public ICollection<Song> Songs { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<MixDb> Mixes { get; set; }
    }
}
