using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using Music_Library_Management_Application.Utilities;
using Music_Library_Management_Application.Services.Interfaces;

namespace Music_Library_Management_Application.Services
{
    public class SongService : ISongService
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SongService(IRepositoryWrapper repoWrapper, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _repoWrapper = repoWrapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

        public async Task<IEnumerable<Song>> GetAllSongsByUserIdAsync(string userId)
        {
            return _repoWrapper.Songs.GetAllByUserId(userId);
        }

        public async Task<Song> GetSongByIdAndUserIdAsync(int id, string userId)
        {
            return _repoWrapper.Songs.GetByIdAndUserId(id, userId);
        }

        public async Task UploadSongsAsync(List<IFormFile> files, string userId)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("No files selected for upload.");

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

            _httpContextAccessor.HttpContext.Session.SetString("UploadedFiles", JsonConvert.SerializeObject(uploadedFiles));
        }

        public SongViewModel AnalyzeSong(SongFileViewModel file)
        {
            string songTitle = string.Empty;
            string songArtist = string.Empty;
            string songAlbum = string.Empty;
            string songLength = string.Empty;
            short songBPM = 0;

            // Using TagLib to extract metadata
            using (var stream = new MemoryStream(file.FileContent))
            using (var tagFile = TagLib.File.Create(new StreamFileAbstraction(file.FileName, stream)))
            {
                songTitle = tagFile.Tag.Title;
                songArtist = tagFile.Tag.FirstPerformer;
                songAlbum = tagFile.Tag.Album;
            }

            // Using NAudio to get the length of the song
            using (var mp3Reader = new Mp3FileReader(new MemoryStream(file.FileContent)))
            {
                songLength = mp3Reader.TotalTime.ToString(@"mm\:ss");
            }

            // Using BPMDetector to get the BPM of the song
            string tempFilePath = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempFilePath, file.FileContent);
                songBPM = BPMDetector.GetBPM(tempFilePath);
            }
            finally
            {
                File.Delete(tempFilePath);
            }

            return new SongViewModel
            {
                FileName = file.FileName,
                SongTitle = songTitle,
                SongArtist = songArtist,
                SongAlbum = songAlbum,
                SongLenght = songLength,
                SongBPM = songBPM
            };
        }

        public async Task<IEnumerable<SongViewModel>> GetUploadedFilesAsync()
        {
            var uploadedFilesJson = _httpContextAccessor.HttpContext.Session.GetString("UploadedFiles");
            var uploadedFiles = uploadedFilesJson != null
                ? JsonConvert.DeserializeObject<List<SongFileViewModel>>(uploadedFilesJson)
                : new List<SongFileViewModel>();

            var songDetails = new List<SongViewModel>();
            foreach (var file in uploadedFiles)
            {
                var songViewModel = AnalyzeSong(file);
                songDetails.Add(songViewModel);
            }

            return songDetails;
        }

        public async Task SaveSongsAsync(List<SongViewModel> songs, string userId)
        {
            if (songs == null || songs.Count == 0)
                throw new ArgumentException("No song details provided.");

            var uploadedFilesJson = _httpContextAccessor.HttpContext.Session.GetString("UploadedFiles");
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
                        SongFile = file.FileContent,
                        UserId = userId
                    };

                    _repoWrapper.Songs.Add(song);
                }
            }
            _httpContextAccessor.HttpContext.Session.Remove("UploadedFiles");
        }

        public async Task<byte[]> DownloadSongAsync(int id, string userId)
        {
            var song = await GetSongByIdAndUserIdAsync(id, userId);
            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            return song.SongFile;
        }

        public async Task<Song> GetSongDetailsAsync(int id, string userId)
        {
            return await GetSongByIdAndUserIdAsync(id, userId);
        }

        public async Task UpdateSongAsync(int id, Song songModel, string userId)
        {
            var song = await GetSongByIdAndUserIdAsync(id, userId);

            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            // Update the values
            song.SongTitle = songModel.SongTitle;
            song.Description = songModel.Description;
            song.SongArtist = songModel.SongArtist;
            song.SongAlbum = songModel.SongAlbum;
            song.SongLenght = songModel.SongLenght;
            song.SongBPM = songModel.SongBPM;

            _repoWrapper.Songs.Update(song);
        }

        public async Task DeleteSongAsync(int id, string userId)
        {
            var song = await GetSongByIdAndUserIdAsync(id, userId);
            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            // Find and delete all SongPlaylist links
            var songPlaylists = _repoWrapper.SongPlaylists.Find(sp => sp.SongId == id).ToList();
            foreach (var songPlaylist in songPlaylists)
            {
                _repoWrapper.SongPlaylists.Delete(songPlaylist);
            }

            // Delete the song
            _repoWrapper.Songs.Delete(song);
        }

        public async Task<FileStreamResult> PlaySongAsync(int id, string userId)
        {
            var song = await GetSongByIdAndUserIdAsync(id, userId);
            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            var fileStream = new MemoryStream(song.SongFile);
            var response = new FileStreamResult(fileStream, "audio/mpeg")
            {
                EnableRangeProcessing = true
            };
            return response;
        }

        public async Task<(byte[] FileContent, string FileName)> GetSongFileAsync(int id, string userId)
        {
            var song = await GetSongByIdAndUserIdAsync(id, userId);
            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            return (song.SongFile, song.SongTitle + ".mp3");
        }
    }
}
