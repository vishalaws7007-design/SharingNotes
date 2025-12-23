using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharingNotes.Models;

namespace SharingNotes.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly AppDbContext db;
        public AnalyticsController(AppDbContext appDbContext )
        {
            db = appDbContext;
        }
        [Authorize]
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Login", "Auth");

            var posts = db.Posts
                .Where(x => x.userName == userName)
                .ToList();

            // Totals
            ViewBag.TotalPosts = posts.Count;
            ViewBag.TotalLikes = posts.Sum(x => x.LikeCount);
            ViewBag.TotalViews = posts.Sum(x => x.ViewCount);

            // Count comments (split by ||)
            ViewBag.TotalComments = posts.Sum(p =>
                string.IsNullOrEmpty(p.Comments)
                    ? 0
                    : p.Comments.Split("||").Length
            );

            return View(posts);
        }


    }
}
