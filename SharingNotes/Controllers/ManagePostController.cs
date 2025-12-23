using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharingNotes.Models;
using System.Text.Json;

namespace SharingNotes.Controllers
{
    public class ManagePostController : Controller
    {
        private readonly AppDbContext _context;
        public ManagePostController( AppDbContext context)
        {
            _context = context;
        }
        // ================= INDEX =================
        [Authorize]
        public IActionResult Index()
        {

            var posts = _context.Posts.Where(x => x.userName.ToLower().Equals(HttpContext.Session.GetString("UserName").ToLower())).ToList();            
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

        [Authorize]
        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Post());
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post model)
        {
            if (ModelState.IsValid)
            {
                var post = new Post
                {
                    
                    Title = model.Title,
                    Description = model.Description,
                    Comments = model.Comments,
                    LikeCount = model.LikeCount,
                    userName = HttpContext.Session.GetString("UserName"),
                    ViewCount = model.ViewCount
                };
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                


                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [Authorize]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var post = _context.Posts.FirstOrDefault(x => x.Id == id);
            if (post == null)
                return NotFound();
            return View(post); // ✅ single Post
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Post model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var post = _context.Posts.FirstOrDefault(x => x.Id == model.Id);
            if (post == null)
                return NotFound();

            post.Title = model.Title;
            post.Description = model.Description;
            post.Modifedon= DateTime.Now.ToString("dd-MMM-yyyy:hh:mm");

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // ================= DELETE =================
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var data=_context.Posts.FirstOrDefault(x => x.Id.Equals(id));
            if(data != null)
            {
                _context.Posts.Remove(data);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
