using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;

namespace Music_Library_Management_Application.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly MyDbContext _context;
        private ISongRepository _songs;
        private IPlaylistRepository _playlists;

        public RepositoryWrapper(MyDbContext context)
        {
            _context = context;
        }

        public ISongRepository Songs
        {
            get
            {
                if (_songs == null)
                {
                    _songs = new SongRepository(_context);
                }

                return _songs;
            }
        }

        public IPlaylistRepository Playlists
        {
            get
            {
                if (_playlists == null)
                {
                    _playlists = new PlaylistRepository(_context);
                }

                return _playlists;
            }
        }
    }
}
