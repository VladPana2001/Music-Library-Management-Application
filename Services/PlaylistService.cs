using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Repositories.Interfaces;
using Music_Library_Management_Application.Services.Interfaces;
using NAudio.Wave;

namespace Music_Library_Management_Application.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public PlaylistService(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylistsByUserIdAsync(string userId)
        {
            return _repoWrapper.Playlists.GetAllByUserId(userId);
        }

        public async Task<IEnumerable<Song>> GetAllSongsByUserIdAsync(string userId)
        {
            return _repoWrapper.Songs.GetAllByUserId(userId);
        }

        public async Task CreatePlaylistAsync(PlaylistCreateViewModel viewModel, string userId)
        {
            var playlist = new Playlist
            {
                PlaylistTitle = viewModel.Playlist.PlaylistTitle,
                PlaylistDescription = viewModel.Playlist.PlaylistDescription,
                UserId = userId,
                SongPlaylists = new List<SongPlaylist>()
            };

            _repoWrapper.Playlists.Add(playlist);

            foreach (var songId in viewModel.SelectedSongIds)
            {
                var songPlaylist = new SongPlaylist
                {
                    SongId = songId,
                    PlaylistId = playlist.Id,
                };
                _repoWrapper.SongPlaylists.Add(songPlaylist);
                playlist.SongPlaylists.Add(songPlaylist);
            }

            _repoWrapper.Playlists.Update(playlist);
        }

        public async Task<PlaylistDetailsViewModel> GetPlaylistDetailsAsync(int playlistId, string userId)
        {
            var playlist = _repoWrapper.Playlists.GetByIdAndUserId(playlistId, userId);
            if (playlist == null)
            {
                return null;
            }

            var songPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == playlist.Id).ToList();
            var songs = songPlaylists.Select(sp => _repoWrapper.Songs.GetById(sp.SongId)).ToList();

            return new PlaylistDetailsViewModel
            {
                Playlist = playlist,
                Songs = songs
            };
        }

        public async Task DeletePlaylistAsync(int playlistId, string userId)
        {
            var playlist = _repoWrapper.Playlists.GetByIdAndUserId(playlistId, userId);
            if (playlist == null)
            {
                return;
            }

            var songPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == playlist.Id).ToList();
            foreach (var songPlaylist in songPlaylists)
            {
                _repoWrapper.SongPlaylists.Delete(songPlaylist);
            }

            _repoWrapper.Playlists.Delete(playlist);
        }

        public async Task<PlaylistCreateViewModel> GetPlaylistForEditAsync(int playlistId, string userId)
        {
            var playlist = _repoWrapper.Playlists.GetByIdAndUserId(playlistId, userId);
            if (playlist == null)
            {
                return null;
            }

            var allUserSongs = _repoWrapper.Songs.GetAllByUserId(userId).ToList();
            var selectedSongIds = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == playlistId).Select(sp => sp.SongId).ToList();

            return new PlaylistCreateViewModel
            {
                Playlist = playlist,
                AllUserSongs = allUserSongs,
                SelectedSongIds = selectedSongIds
            };
        }

        public async Task UpdatePlaylistAsync(int playlistId, PlaylistCreateViewModel viewModel, string userId)
        {
            var existingPlaylist = _repoWrapper.Playlists.GetByIdAndUserId(playlistId, userId);
            if (existingPlaylist == null)
            {
                return;
            }

            existingPlaylist.PlaylistTitle = viewModel.Playlist.PlaylistTitle;
            existingPlaylist.PlaylistDescription = viewModel.Playlist.PlaylistDescription;

            _repoWrapper.Playlists.Update(existingPlaylist);

            var existingSongPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == playlistId).ToList();
            foreach (var songPlaylist in existingSongPlaylists)
            {
                _repoWrapper.SongPlaylists.Delete(songPlaylist);
            }

            foreach (var songId in viewModel.SelectedSongIds)
            {
                var songPlaylist = new SongPlaylist
                {
                    SongId = songId,
                    PlaylistId = existingPlaylist.Id,
                };
                _repoWrapper.SongPlaylists.Add(songPlaylist);
            }
        }

        public async Task<PlaylistFile> GenerateCombinedAudioFileForPlaylistAsync(int playlistId, string userId)
        {
            var playlist = await GetPlaylistDetailsAsync(playlistId, userId);
            if (playlist == null || !playlist.Songs.Any())
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var waveFileWriter = new WaveFileWriter(memoryStream, new WaveFormat()))
                {
                    foreach (var song in playlist.Songs)
                    {
                        using (var mp3Reader = new Mp3FileReader(new MemoryStream(song.SongFile)))
                        {
                            if (waveFileWriter.WaveFormat.SampleRate != mp3Reader.WaveFormat.SampleRate ||
                                waveFileWriter.WaveFormat.Channels != mp3Reader.WaveFormat.Channels)
                            {
                                // Convert to the same format if necessary
                                using (var resampler = new MediaFoundationResampler(mp3Reader, waveFileWriter.WaveFormat))
                                {
                                    resampler.ResamplerQuality = 60; // Quality of the conversion
                                    byte[] buffer = new byte[1024];
                                    int bytesRead;
                                    while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        waveFileWriter.Write(buffer, 0, bytesRead);
                                    }
                                }
                            }
                            else
                            {
                                byte[] buffer = new byte[1024];
                                int bytesRead;
                                while ((bytesRead = mp3Reader.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    waveFileWriter.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                    }
                }

                return new PlaylistFile
                {
                    FileName = $"{playlist.Playlist.PlaylistTitle}.mp3",
                    FileBytes = memoryStream.ToArray()
                };
            }
        }
    }
}
