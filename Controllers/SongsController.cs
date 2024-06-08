using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Services.Interfaces;
using System.Threading.Tasks;

namespace Music_Library_Management_Application.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private readonly ISongService _songService;
        private readonly UserManager<User> _userManager;

        public SongsController(ISongService songService, UserManager<User> userManager)
        {
            _songService = songService;
            _userManager = userManager;
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var songs = await _songService.GetAllSongsByUserIdAsync(user.Id);
            return View(songs);
        }

        [HttpGet]
        public IActionResult UploadSongs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadSongs(List<IFormFile> files)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                await _songService.UploadSongsAsync(files, user.Id);
                return RedirectToAction("EnterSongDetails");
            }
            catch (ArgumentException ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> EnterSongDetails()
        {
            var songDetails = await _songService.GetUploadedFilesAsync();
            return View(songDetails);
        }

        [HttpPost]
        public async Task<IActionResult> EnterSongDetails(List<SongViewModel> songs)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                await _songService.SaveSongsAsync(songs, user.Id);
                ViewBag.Message = "Songs uploaded and saved successfully.";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ViewBag.Message = ex.Message;
                return View(songs);
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                var (fileContent, fileName) = await _songService.GetSongFileAsync(id, user.Id);
                return File(fileContent, "application/octet-stream", fileName);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var song = await _songService.GetSongDetailsAsync(id.Value, user.Id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var song = await _songService.GetSongDetailsAsync(id.Value, user.Id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SongTitle,Description,SongArtist,SongAlbum,SongLenght,SongBPM")] Song songModel)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                await _songService.UpdateSongAsync(id, songModel, user.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var song = await _songService.GetSongDetailsAsync(id.Value, user.Id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                await _songService.DeleteSongAsync(id, user.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Play(int id)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                var response = await _songService.PlaySongAsync(id, user.Id);
                return response;
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
