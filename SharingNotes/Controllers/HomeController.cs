using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SharingNotes.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // ================= READ MORE (ONLY ONE VIEW PER USER) =================
        [HttpGet]
        public IActionResult ReadMore(int id)
        {
            string viewedKey = $"Post_{id}_Viewed";
            string viewCountKey = $"Post_{id}_Views";

            // ? Increment view ONLY ONCE per session
            if (HttpContext.Session.GetString(viewedKey) == null)
            {
                int views = HttpContext.Session.GetInt32(viewCountKey) ?? 0;
                HttpContext.Session.SetInt32(viewCountKey, views + 1);
                HttpContext.Session.SetString(viewedKey, "true");
            }

            // ?? Content
            if (id == 1)
            {
                ViewBag.Title = "HTML";
                ViewBag.Description =
                    "HTML (HyperText Markup Language) is the foundational language used to structure content on the web.";
            }
            else
            {
                ViewBag.Title = "CSS";
                ViewBag.Description =
                    "CSS (Cascading Style Sheets) controls the appearance and layout of web pages.";
            }

            // ?? Load counts
            ViewBag.Views = HttpContext.Session.GetInt32(viewCountKey) ?? 0;
            ViewBag.Likes = HttpContext.Session.GetInt32($"Post_{id}_Likes") ?? 0;

            // ?? Flags for UI
            ViewBag.AlreadyLiked =
                HttpContext.Session.GetString($"Post_{id}_Liked") != null;

            // ?? Load comments
            ViewBag.CommentsJson =
                HttpContext.Session.GetString($"Post_{id}_Comments");

            return View();
        }

        // ================= LIKE (ONLY ONE LIKE PER USER) =================
        [HttpPost]
        public IActionResult Like(int id)
        {
            string likedKey = $"Post_{id}_Liked";
            string likeCountKey = $"Post_{id}_Likes";

            // ? Allow only one like per session
            if (HttpContext.Session.GetString(likedKey) == null)
            {
                int likes = HttpContext.Session.GetInt32(likeCountKey) ?? 0;
                HttpContext.Session.SetInt32(likeCountKey, likes + 1);
                HttpContext.Session.SetString(likedKey, "true");
            }

            return RedirectToAction(nameof(ReadMore), new { id });
        }

        // ================= ADD COMMENT =================
        [HttpPost]
        public IActionResult AddComment(int id, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
                return RedirectToAction(nameof(ReadMore), new { id });

            string key = $"Post_{id}_Comments";

            var json = HttpContext.Session.GetString(key);
            var comments = string.IsNullOrEmpty(json)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(json);

            comments.Add(commentText);

            HttpContext.Session.SetString(
                key,
                JsonSerializer.Serialize(comments)
            );

            return RedirectToAction(nameof(ReadMore), new { id });
        }
    }
}
