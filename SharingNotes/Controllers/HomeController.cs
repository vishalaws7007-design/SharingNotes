using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharingNotes.Models;

namespace SharingNotes.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReadMore()
        {
            return View();
        }
    }
}
