using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;

namespace Music_Library_Management_Application.Repositories
{
    public class SongPlaylistRepository : Repository<SongPlaylist>, ISongPlaylistRepository
    {
        public SongPlaylistRepository(MyDbContext context) : base(context) { }


    }
}
