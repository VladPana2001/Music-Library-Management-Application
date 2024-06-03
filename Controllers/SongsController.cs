using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories;
using Music_Library_Management_Application.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Music_Library_Management_Application.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly UserManager<User> _userManager;

        public SongsController(IRepositoryWrapper repoWrapper, UserManager<User> userManager)
        {
            _repoWrapper = repoWrapper;
            _userManager = userManager;
        }
        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var songs = _repoWrapper.Songs.GetAllByUserId(user.Id);
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
            if (files == null || files.Count == 0)
            {
                ViewBag.Message = "No files selected for upload.";
                return View();
            }

            var uploadedFiles = new List<SongFileViewModel>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        uploadedFiles.Add(new SongFileViewModel
                        {
                            FileName = file.FileName,
                            FileContent = memoryStream.ToArray()
                        });
                    }
                }
            }

            HttpContext.Session.SetString("UploadedFiles", JsonConvert.SerializeObject(uploadedFiles));
            return RedirectToAction("EnterSongDetails");
        }

        [HttpGet]
        public IActionResult EnterSongDetails()
        {
            var uploadedFilesJson = HttpContext.Session.GetString("UploadedFiles");
            var uploadedFiles = uploadedFilesJson != null
                ? JsonConvert.DeserializeObject<List<SongFileViewModel>>(uploadedFilesJson)
                : new List<SongFileViewModel>();

            var songDetails = new List<SongViewModel>();
            foreach (var file in uploadedFiles)
            {
                songDetails.Add(new SongViewModel
                {
                    FileName = file.FileName
                });
            }

            return View(songDetails);
        }
        [HttpPost]
        public async Task<IActionResult> EnterSongDetails(List<SongViewModel> songs)
        {
            if (songs == null || songs.Count == 0)
            {
                ViewBag.Message = "No song details provided.";
                return View(songs);
            }

            var uploadedFilesJson = HttpContext.Session.GetString("UploadedFiles");
            var uploadedFiles = uploadedFilesJson != null
                ? JsonConvert.DeserializeObject<List<SongFileViewModel>>(uploadedFilesJson)
                : new List<SongFileViewModel>();

            var user = await GetCurrentUserAsync();

            foreach (var songViewModel in songs)
            {
                var file = uploadedFiles.Find(f => f.FileName == songViewModel.FileName);
                if (file != null)
                {
                    var song = new Song
                    {
                        SongTitle = songViewModel.SongTitle,
                        Description = songViewModel.Description,
                        SongArtist = songViewModel.SongArtist,
                        SongAlbum = songViewModel.SongAlbum,
                        SongLenght = songViewModel.SongLenght,
                        SongBPM = songViewModel.SongBPM,
                        SongFile = file.FileContent,
                        UserId = user.Id
                    };

                    _repoWrapper.Songs.Add(song);
                }
            }
            HttpContext.Session.Remove("UploadedFiles");
            ViewBag.Message = "Songs uploaded and saved successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Download(int id)
        {
            var user = await GetCurrentUserAsync();
            var song = _repoWrapper.Songs.GetByIdAndUserId(id, user.Id);
            if (song == null)
            {
                return NotFound();
            }

            return File(song.SongFile, "application/octet-stream", song.SongTitle + ".mp3");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var song = _repoWrapper.Songs.GetByIdAndUserId(id.Value, user.Id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var song = _repoWrapper.Songs.GetByIdAndUserId(id.Value, user.Id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        // POST: Songs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SongTitle,Description,SongArtist,SongAlbum,SongLenght,SongBPM")] Song songModel)
        {
            if (id != songModel.Id)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var song = _repoWrapper.Songs.GetByIdAndUserId(id, user.Id);

            if (song == null)
            {
                return NotFound();
            }

            // Update the values
            song.SongTitle = songModel.SongTitle;
            song.Description = songModel.Description;
            song.SongArtist = songModel.SongArtist;
            song.SongAlbum = songModel.SongAlbum;
            song.SongLenght = songModel.SongLenght;
            song.SongBPM = songModel.SongBPM;

            try
            {
                _repoWrapper.Songs.Update(song);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(songModel.Id, user.Id))
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

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var song = _repoWrapper.Songs.GetByIdAndUserId(id.Value, user.Id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetCurrentUserAsync();
            var song = _repoWrapper.Songs.GetByIdAndUserId(id, user.Id);
            _repoWrapper.Songs.Delete(song);
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id, string userId)
        {
            return _repoWrapper.Songs.Find(e => e.Id == id && e.UserId == userId).Any();
        }

    }
}
