using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;
using NAudio.Mixer;

namespace Music_Library_Management_Application.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly MyDbContext _context;
        private ISongRepository _songs;
        private IPlaylistRepository _playlists;
        private ISongPlaylistRepository _songPlaylists;
        private IMixRepository _mixes;
        private IMixSongRepository _mixSongs;

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

        public ISongPlaylistRepository SongPlaylists
        {
            get
            {
                if (_songPlaylists == null)
                {
                    _songPlaylists = new SongPlaylistRepository(_context);
                }

                return _songPlaylists;
            }
        }

        public IMixRepository Mixes
        {
            get
            {
                if (_mixes == null)
                {
                    _mixes = new MixRepository(_context);
                }

                return _mixes;
            }
        }

        public IMixSongRepository MixSongs
        {
            get
            {
                if (_mixSongs == null)
                {
                    _mixSongs = new MixSongRepository(_context);
                }

                return _mixSongs;
            }
        }
    }
}
