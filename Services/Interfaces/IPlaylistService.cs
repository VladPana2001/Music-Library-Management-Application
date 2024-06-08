using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Models;

namespace Music_Library_Management_Application.Services.Interfaces
{
    public interface IPlaylistService
    {
        Task<IEnumerable<Playlist>> GetAllPlaylistsByUserIdAsync(string userId);
        Task<IEnumerable<Song>> GetAllSongsByUserIdAsync(string userId);
        Task CreatePlaylistAsync(PlaylistCreateViewModel viewModel, string userId);
        Task<PlaylistDetailsViewModel> GetPlaylistDetailsAsync(int playlistId, string userId);
        Task DeletePlaylistAsync(int playlistId, string userId);
        Task<PlaylistCreateViewModel> GetPlaylistForEditAsync(int playlistId, string userId);
        Task UpdatePlaylistAsync(int playlistId, PlaylistCreateViewModel viewModel, string userId);
        Task<byte[]> GenerateCombinedAudioFileAsync(int playlistId, string userId);
    }
}
