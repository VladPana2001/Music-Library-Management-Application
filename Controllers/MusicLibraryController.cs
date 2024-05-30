using Microsoft.AspNetCore.Mvc;

namespace Music_Library_Management_Application.Controllers
{
    public class MusicLibraryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
