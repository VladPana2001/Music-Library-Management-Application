using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Services.Interfaces;
using Music_Library_Management_Application.Repositories.Interfaces;

namespace Music_Library_Management_Application.Controllers
{
    [Authorize]
    public class MixController : Controller
    {
        private readonly IMixService _mixService;
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryWrapper _repoWrapper;

        public MixController
            (IMixService mixService,
            UserManager<User> userManager,
            IRepositoryWrapper repositoryWrapper)
        {
            _mixService = mixService;
            _userManager = userManager;
            _repoWrapper = repositoryWrapper;
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var mixes = _repoWrapper.Mixes.GetAllByUserId(user.Id);

            return View(mixes);
        }

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
                Songs = model.MixSongs,
                LimiterThreshold = model.LimiterThreshold
            };

            var fileBytes = await _mixService.GenerateMixAsync(mix, user.Id);

            return File(fileBytes, "audio/mpeg", $"{mix.Name}.mp3");
        }
    }
}
