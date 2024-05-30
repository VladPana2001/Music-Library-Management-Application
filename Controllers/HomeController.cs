using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Models;
using System.Diagnostics;

namespace Music_Library_Management_Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;

        public HomeController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    var musicFile = new MusicFile
                    {
                        FileName = file.FileName,
                        Data = memoryStream.ToArray()
                    };

                    _context.MusicFiles.Add(musicFile);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var files = await _context.MusicFiles.ToListAsync();
            return View(files);
        }

        public async Task<IActionResult> Download(int id)
        {
            var file = await _context.MusicFiles.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            return File(file.Data, "application/octet-stream", file.FileName);
        }

        public async Task<IActionResult> Play(int id)
        {
            var file = await _context.MusicFiles.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            var fileStream = new MemoryStream(file.Data);
            var response = File(fileStream, "audio/mpeg", enableRangeProcessing: true);
            return response;
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}