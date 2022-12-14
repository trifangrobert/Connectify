using connectify.Data;
using connectify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connectify.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index()
        {
            var posts = db.Posts.Include("User");

            ViewBag.Posts = posts;

            return View();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            var post = db.Posts.Find(id);

            ViewBag.User = _userManager.GetUserAsync(User);

            return View(post);
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New()
        {
            Post post = new Post();
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New(Post post)
        {
            if (ModelState.IsValid)
            {
                post.Date = DateTime.Now;
                post.UserId = _userManager.GetUserId(User);
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id)
        {
            var post = db.Posts.Find(id);

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "Cannot edit this post. You are not the author.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id, Post reqpost)
        {
            Post post = db.Posts.Find(id);
            
            if (ModelState.IsValid)
            {

                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    post.Date = DateTime.Now;
                    post.Title = reqpost.Title;
                    post.Content = reqpost.Content;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot edit this post. You are not the author.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(post);
            }
        }
        
        

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Delete(int id)
        {
            var post = db.Posts.Find(id);
            if (ModelState.IsValid)
            {
                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.Posts.Remove(post);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot delete this post. You are not the author.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(post);
            }

        }
    }
}
