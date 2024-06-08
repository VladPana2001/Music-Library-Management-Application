using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;
using Music_Library_Management_Application.Services.Interfaces;
using NAudio.Wave;

namespace Music_Library_Management_Application.Controllers
{
    [Authorize]
    public class PlaylistsController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private readonly UserManager<User> _userManager;

        public PlaylistsController(IPlaylistService playlistService, UserManager<User> userManager)
        {
            _playlistService = playlistService;
            _userManager = userManager;
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var playlists = await _playlistService.GetAllPlaylistsByUserIdAsync(user.Id);
            return View(playlists);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            var allUserSongs = await _playlistService.GetAllSongsByUserIdAsync(user.Id);
            var viewModel = new PlaylistCreateViewModel
            {
                Playlist = new Playlist(),
                AllUserSongs = allUserSongs.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaylistCreateViewModel viewModel)
        {
            var user = await GetCurrentUserAsync();
            await _playlistService.CreatePlaylistAsync(viewModel, user.Id);
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
            var viewModel = await _playlistService.GetPlaylistDetailsAsync(id.Value, user.Id);
            if (viewModel == null)
            {
                return NotFound();
            }

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
            var viewModel = await _playlistService.GetPlaylistDetailsAsync(id.Value, user.Id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetCurrentUserAsync();
            await _playlistService.DeletePlaylistAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var viewModel = await _playlistService.GetPlaylistForEditAsync(id.Value, user.Id);
            if (viewModel == null)
            {
                return NotFound();
            }

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
            await _playlistService.UpdatePlaylistAsync(id, viewModel, user.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var user = await GetCurrentUserAsync();
            var fileBytes = await _playlistService.GenerateCombinedAudioFileAsync(id, user.Id);

            if (fileBytes == null)
            {
                return NotFound();
            }

            var playlist = await _playlistService.GetPlaylistDetailsAsync(id, user.Id);
            var outputFileName = $"{playlist.Playlist.PlaylistTitle}.mp3";

            return File(fileBytes, "audio/mpeg", outputFileName);
        }
    }
}
