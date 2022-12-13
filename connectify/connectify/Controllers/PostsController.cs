using connectify.Data;
using connectify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace connectify.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        public PostsController(
            ApplicationDbContext context
            )
        {
            db = context;
        }
        public IActionResult Index()
        {
            var posts = db.Posts.ToList();

            ViewBag.Posts = posts;

            return View();
        }

        public IActionResult Show(int id)
        {
            var post = db.Posts.Find(id);

            ViewBag.Post = post;

            return View(post);
        }

        public IActionResult New()
        {
            Post post = new Post();
            return View(post);
        }

        [HttpPost]
        public IActionResult New(Post post)
        {
            try
            {
                post.Date = DateTime.Now;
                db.Posts.Add(post);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(post);
            }
        }

        public IActionResult Edit(int id)
        {
            var post = db.Posts.Find(id);

            ViewBag.Post = post;

            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(int id, Post post)
        {
            try
            {
                db.Posts.Update(post);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(post);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var post = db.Posts.Find(id);
                db.Posts.Remove(post);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
