using Microsoft.AspNetCore.Mvc;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;

public interface ISongService
{
    Task<IEnumerable<Song>> GetAllSongsByUserIdAsync(string userId);
    Task<Song> GetSongByIdAndUserIdAsync(int id, string userId);
    Task UploadSongsAsync(List<IFormFile> files, string userId);
    Task<IEnumerable<SongViewModel>> GetUploadedFilesAsync();
    Task SaveSongsAsync(List<SongViewModel> songs, string userId);
    Task<byte[]> DownloadSongAsync(int id, string userId);
    Task<Song> GetSongDetailsAsync(int id, string userId);
    Task UpdateSongAsync(int id, Song songModel, string userId);
    Task DeleteSongAsync(int id, string userId);
    Task<FileStreamResult> PlaySongAsync(int id, string userId);
    Task<(byte[] FileContent, string FileName)> GetSongFileAsync(int id, string userId);
}
