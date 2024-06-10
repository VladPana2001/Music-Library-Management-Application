using Music_Library_Management_Application.Models;

namespace Music_Library_Management_Application.Services.Interfaces
{
    public interface IMixService
    {
        Task<MixCreateViewModel> GetMixCreateViewModelAsync(string userId);
        Task<byte[]> GenerateMixAsync(Mix mix, string userId);
    }
}
