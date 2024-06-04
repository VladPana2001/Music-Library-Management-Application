using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;

namespace Music_Library_Management_Application.Controllers
{
    [Authorize]
    public class PlaylistsController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly UserManager<User> _userManager;

        public PlaylistsController(IRepositoryWrapper repoWrapper, UserManager<User> userManager)
        {
            _repoWrapper = repoWrapper;
            _userManager = userManager;
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var playlists = _repoWrapper.Playlists.GetAllByUserId(user.Id);
            return View(playlists);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            var allUserSongs = _repoWrapper.Songs.GetAllByUserId(user.Id).ToList();
            var viewModel = new PlaylistCreateViewModel
            {
                Playlist = new Playlist(),
                AllUserSongs = allUserSongs
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaylistCreateViewModel viewModel)
        {

            var user = await GetCurrentUserAsync();

            // Create a new Playlist instance
            var playlist = new Playlist
            {
                PlaylistTitle = viewModel.Playlist.PlaylistTitle,
                PlaylistDescription = viewModel.Playlist.PlaylistDescription,
                UserId = user.Id,
                User = user,
                SongPlaylists = new List<SongPlaylist>()
            };

            _repoWrapper.Playlists.Add(playlist);

            // Create SongPlaylists to link the songs to the playlist
            foreach (var songId in viewModel.SelectedSongIds)
            {
                var songPlaylist = new SongPlaylist
                {   
                    SongId = songId,
                    Song = _repoWrapper.Songs.GetById(songId),   
                    PlaylistId = playlist.Id,
                    Playlist = playlist
                };
                _repoWrapper.SongPlaylists.Add(songPlaylist);
                playlist.SongPlaylists.Add(songPlaylist);
            }

            _repoWrapper.Playlists.Update(playlist);
           

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var playlist = _repoWrapper.Playlists.GetByIdAndUserId(id.Value, user.Id);
            if (playlist == null)
            {
                return NotFound();
            }

            var songPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == playlist.Id).ToList();
            var songs = songPlaylists.Select(sp => _repoWrapper.Songs.GetById(sp.SongId)).ToList();

            var viewModel = new PlaylistDetailsViewModel
            {
                Playlist = playlist,
                Songs = songs
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var playlist = _repoWrapper.Playlists.GetByIdAndUserId(id.Value, user.Id);
            if (playlist == null)
            {
                return NotFound();
            }

            var songPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == playlist.Id).ToList();
            var songs = songPlaylists.Select(sp => _repoWrapper.Songs.GetById(sp.SongId)).ToList();

            var viewModel = new PlaylistDetailsViewModel
            {
                Playlist = playlist,
                Songs = songs
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetCurrentUserAsync();
            var playlist = _repoWrapper.Playlists.GetByIdAndUserId(id, user.Id);
            if (playlist == null)
            {
                return NotFound();
            }

            var songPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == playlist.Id).ToList();
            foreach (var songPlaylist in songPlaylists)
            {
                _repoWrapper.SongPlaylists.Delete(songPlaylist);
            }

            _repoWrapper.Playlists.Delete(playlist);

            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistExists(int id, string userId)
        {
            return _repoWrapper.Playlists.Find(e => e.Id == id && e.UserId == userId).Any();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var playlist = _repoWrapper.Playlists.GetByIdAndUserId(id.Value, user.Id);
            if (playlist == null)
            {
                return NotFound();
            }

            var allUserSongs = _repoWrapper.Songs.GetAllByUserId(user.Id).ToList();
            var selectedSongIds = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == id.Value).Select(sp => sp.SongId).ToList();

            var viewModel = new PlaylistCreateViewModel
            {
                Playlist = playlist,
                AllUserSongs = allUserSongs,
                SelectedSongIds = selectedSongIds
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlaylistCreateViewModel viewModel)
        {
            if (id != viewModel.Playlist.Id)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var existingPlaylist = _repoWrapper.Playlists.GetByIdAndUserId(id, user.Id);

            if (existingPlaylist == null)
            {
                return NotFound();
            }

            existingPlaylist.PlaylistTitle = viewModel.Playlist.PlaylistTitle;
            existingPlaylist.PlaylistDescription = viewModel.Playlist.PlaylistDescription;

            try
            {
                // Update the Playlist
                _repoWrapper.Playlists.Update(existingPlaylist);

                // Remove existing SongPlaylists
                var existingSongPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.PlaylistId == id).ToList();
                foreach (var songPlaylist in existingSongPlaylists)
                {
                    _repoWrapper.SongPlaylists.Delete(songPlaylist);
                }

                // Add the new SongPlaylists
                foreach (var songId in viewModel.SelectedSongIds)
                {
                    var songPlaylist = new SongPlaylist
                    {
                        SongId = songId,
                        PlaylistId = existingPlaylist.Id,
                        Song = _repoWrapper.Songs.GetById(songId),
                        Playlist = existingPlaylist
                    };

                    _repoWrapper.SongPlaylists.Add(songPlaylist);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(viewModel.Playlist.Id, user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
