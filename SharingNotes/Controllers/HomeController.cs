using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharingNotes.Models;
using System.Text.Json;

namespace SharingNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public IActionResult Index()
        {
            var posts = _context.Posts.ToList();
            return View(posts);
        }
        // ================= READ MORE (ONLY ONE VIEW PER USER) =================
        [HttpGet]
        public IActionResult ReadMore(string id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return NotFound();
            string viewCountKey = $"Post_{id}_Views";
            string viewedKey = $"Post_{id}_Viewed";
            if (HttpContext.Session.GetString(viewedKey) == null)
            {
                int views = HttpContext.Session.GetInt32(viewCountKey) ?? 0;
                HttpContext.Session.SetString(viewedKey, "true");
                post.ViewCount += 1;
                _context.SaveChanges();
            }
            ViewBag.Id = id;
            ViewBag.Title = post.Title;
            ViewBag.Description = post.Description;
            ViewBag.Views = post.ViewCount;
            ViewBag.Likes = post.LikeCount;
            ViewBag.AlreadyLiked = HttpContext.Session.GetString($"Post_{id}_Liked") != null;
            // Parse comments
            var commentList = new List<dynamic>();
            if (!string.IsNullOrEmpty(post.Comments))
            {
                var comments = post.Comments.Split("||");
                foreach (var c in comments)
                {
                    var parts = c.Split("~");
                    if (parts.Length == 3)
                    {
                        commentList.Add(new
                        {
                            UserName = parts[0],
                            Text = parts[1],
                            Date = parts[2]
                        });
                    }
                }
            }
            ViewBag.Comments = commentList;
            return View();
        }
        // ================= LIKE (ONLY ONE LIKE PER USER) =================
        [Authorize]
        [HttpPost]
        public IActionResult Like(string id)
        {
            string likedKey = $"Post_{id}_Liked";
            string likeCountKey = $"Post_{id}_Likes";
            var post = _context.Posts.FirstOrDefault(x => x.Id.Equals(id));
            // ? Allow only one like per session
            if ((HttpContext.Session.GetString(likedKey) == null))
            {
                post.LikeCount = post.LikeCount + 1;
                _context.SaveChanges();
                HttpContext.Session.SetString(likedKey, "true");

            }

            return RedirectToAction(nameof(ReadMore), new { id });
        }

        // ================= ADD COMMENT =================
        [Authorize]
        [HttpPost]
        public IActionResult AddComment(string id, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
                return RedirectToAction(nameof(ReadMore), new { id });
            var post = _context.Posts.FirstOrDefault(x => x.Id == id);
            if (post != null)
            {
                string userName = HttpContext.Session.GetString("UserName") ?? "Guest";
                string dateTime = DateTime.Now.ToString("dd MMM yyyy hh:mm tt");

                string newComment = $"{userName}~{commentText}~{dateTime}";

                if (string.IsNullOrEmpty(post.Comments))
                    post.Comments = newComment;
                else
                    post.Comments += "||" + newComment;

                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ReadMore), new { id });
        }
        [HttpGet]
        public IActionResult Search(string title)
        {
            var results = _context.Posts
                .Where(p => p.Title.Contains(title) || p.Description.Contains(title))
                .ToList();

            return View(results);
        }



    }
}
