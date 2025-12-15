using Microsoft.AspNetCore.Mvc;
using SharingNotes.Models;
using System.Text.Json;

namespace SharingNotes.Controllers
{
    public class ManagePostController : Controller
    {
        // ================= INDEX =================
        public IActionResult Index()
        {
            var posts = new List<Post>
            {
                new Post
                {
                    Id = 1,
                    Title = "HTML",
                    Description = "With supporting text below as a natural lead-in to additional content."
                },
                new Post
                {
                    Id = 2,
                    Title = "CSS",
                    Description = "CSS is used to style and layout web pages."
                }
            };

            // Load likes & comments from session
            foreach (var post in posts)
            {
                post.LikeCount =
                    HttpContext.Session.GetInt32($"Post_{post.Id}_Likes") ?? 0;

                post.Comments =
                    HttpContext.Session.GetString($"Post_{post.Id}_Comments");
            }

            return View(posts);
        }

        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Post());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Post model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["Title"] = model.Title;
            TempData["Description"] = model.Description;

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Post post;

            if (id == 1)
            {
                post = new Post
                {
                    Id = 1,
                    Title = "HTML",
                    Description = "HTML is the backbone of the web."
                };
            }
            else
            {
                post = new Post
                {
                    Id = id,
                    Title = "CSS",
                    Description = "CSS styles the web."
                };
            }

            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(Post model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["Title"] = model.Title;
            TempData["Description"] = model.Description;

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // TEMP delete (no DB)
            HttpContext.Session.Remove($"Post_{id}_Likes");
            HttpContext.Session.Remove($"Post_{id}_Comments");

            return RedirectToAction(nameof(Index));
        }

        // ================= LIKE =================
        [HttpPost]
        public IActionResult Like(int id)
        {
            string key = $"Post_{id}_Likes";
            int likes = HttpContext.Session.GetInt32(key) ?? 0;

            HttpContext.Session.SetInt32(key, likes + 1);

            return RedirectToAction(nameof(Index));
        }

        // ================= COMMENT =================
        [HttpPost]
        public IActionResult AddComment(int postId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
                return RedirectToAction(nameof(Index));

            string key = $"Post_{postId}_Comments";

            var json = HttpContext.Session.GetString(key);
            var comments = string.IsNullOrEmpty(json)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(json);

            comments.Add(commentText);

            HttpContext.Session.SetString(
                key,
                JsonSerializer.Serialize(comments)
            );

            return RedirectToAction(nameof(Index));
        }
    }
}
