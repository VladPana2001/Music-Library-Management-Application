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

            if (fileBytes == null)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var mix = _repoWrapper.Mixes.GetById(id);
            if (mix == null || mix.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return File(mix.MixFile, "audio/mpeg", $"{mix.Title}.mp3");
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var mix = _repoWrapper.Mixes.GetByIdAndUserId(id, userId);

            if (mix == null)
            {
                return NotFound();
            }

            var mixSongs = _repoWrapper.MixSongs.Find(ms => ms.MixDbId == id).ToList();

            var mixDetails = new MixDetailsViewModel
            {
                Id = mix.Id,
                Title = mix.Title,
                Description = mix.Description,
                LimiterThreshold = mix.LimiterThreshold,
                Songs = mixSongs
            };

            return View(mixDetails);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var mix = _repoWrapper.Mixes.GetByIdAndUserId(id, userId);

            if (mix == null)
            {
                return NotFound();
            }

            var mixSongs = _repoWrapper.MixSongs.Find(ms => ms.MixDbId == id).ToList();

            var mixDetails = new MixDetailsViewModel
            {
                Id = mix.Id,
                Title = mix.Title,
                Description = mix.Description,
                LimiterThreshold = mix.LimiterThreshold,
                Songs = mixSongs
            };

            return View(mixDetails);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var mix = _repoWrapper.Mixes.GetById(id);
            if (mix == null || mix.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            _repoWrapper.Mixes.Delete(mix);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Stream(int id)
        {
            var mix = _repoWrapper.Mixes.GetById(id);
            if (mix == null || mix.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return File(mix.MixFile, "audio/mpeg", enableRangeProcessing: true);
        }
    }
}
