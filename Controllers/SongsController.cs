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

    }
}
