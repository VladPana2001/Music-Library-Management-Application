using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Services.Interfaces
{
    public interface IMixService
    {
        Task<MixCreateViewModel> GetMixCreateViewModelAsync(string userId);
        Task<byte[]> GenerateMixAsync(Mix mix, string userId);
        public void CreateMixRecord(MixDb mixDb, List<MixSongDb> mixSongsDb);
    }
}
