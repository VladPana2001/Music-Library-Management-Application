using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using System.Diagnostics;

namespace Music_Library_Management_Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("Index");
            }

            return View("Landing");
        }

    }
}