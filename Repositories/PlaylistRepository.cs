using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;

namespace Music_Library_Management_Application.Repositories
{
    public class PlaylistRepository : Repository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(MyDbContext context) : base(context) {}
    }
}
