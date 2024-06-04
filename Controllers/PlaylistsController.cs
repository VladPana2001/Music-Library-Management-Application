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


    }
}
