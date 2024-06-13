using Microsoft.AspNetCore.Mvc;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Services.Interfaces
{
    public interface IMixService
    {
        Task<MixCreateViewModel> GetMixCreateViewModelAsync(string userId);
        Task<byte[]> GenerateMixAsync(Mix mix, string userId);
        void CreateMixRecord(MixDb mixDb, List<MixSongDb> mixSongsDb);
        Task<FileStreamResult> PlayMixAsync(int id, string userId);
        Task<MixDb> GetMixByIdAndUserIdAsync(int id, string userId);
        Task<byte[]> PreviewSongAsync(int songId, string userId, double startTime, double endTime, double fadeInDuration, double fadeOutDuration, double volume);
    }
}
