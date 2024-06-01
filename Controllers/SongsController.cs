using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories;
using Music_Library_Management_Application.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Music_Library_Management_Application.Controllers
{
    public class SongsController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public SongsController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        public IActionResult Index()
        {
            var songs = _repoWrapper.Songs.GetAll();
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
                        SongFile = file.FileContent
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
            var song =  _repoWrapper.Songs.GetById(id);
            if (song == null)
            {
                return NotFound();
            }

            return File(song.SongFile, "application/octet-stream", song.SongTitle + ".mp3");
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = _repoWrapper.Songs.GetById(id.Value);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Edit/5
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song =_repoWrapper.Songs.GetById(id.Value);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        // POST: Songs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("SongId,SongTitle,Description,SongArtist,SongAlbum,SongLenght,SongBPM")] Song songModel)
        {
            if (id != songModel.SongId)
            {
                return NotFound();
            }

            var song = _repoWrapper.Songs.GetById(id);

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
                if (!SongExists(songModel.SongId))
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
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = _repoWrapper.Songs.GetById(id.Value);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var song = _repoWrapper.Songs.GetById(id);
            _repoWrapper.Songs.Delete(song);
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _repoWrapper.Songs.Find(e => e.SongId == id).Any();
        }

    }
}
