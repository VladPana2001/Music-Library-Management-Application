using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Services.Interfaces;

namespace Music_Library_Management_Application.Controllers
{
    [Authorize]
    public class MixController : Controller
    {
        private readonly IMixService _mixService;
        private readonly UserManager<User> _userManager;

        public MixController(IMixService mixService, UserManager<User> userManager)
        {
            _mixService = mixService;
            _userManager = userManager;
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            var model = await _mixService.GetMixCreateViewModelAsync(user.Id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MixCreateViewModel model)
        {
            var user = await GetCurrentUserAsync();

            var mix = new Mix
            {
                Name = model.MixName,
                Description = model.MixDescription,
                Songs = model.MixSongs
            };

            var fileBytes = await _mixService.GenerateMixAsync(mix, user.Id);

            return File(fileBytes, "audio/mpeg", $"{mix.Name}.mp3");
        }
    }
}
