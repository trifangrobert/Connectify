using connectify.Data;
using connectify.Models;
using Ganss.Xss;
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
            int pageSize = 4;

            var posts = db.Posts.Include("User").OrderByDescending(p => p.Date);

            int totalItems = posts.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                List<string> userIds = db.Users.Where(u => u.UserName.Contains(search)).Select(u => u.Id).ToList();
            }

            ViewBag.SearchString = search;

            if (currentPage > 1)
            {
                offset = (currentPage - 1) * pageSize;
            }

            var paginatedPosts = posts.Skip(offset).Take(pageSize);

            ViewBag.lastPage = Math.Ceiling((double)totalItems / pageSize);

            ViewBag.Posts = paginatedPosts;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index?page";
            }

            return View();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("User").Include("Comments").Include("Comments.User").Where(pst => pst.Id == id).First();

            SetAccessRights();

            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }

            else
            {
                Post post = db.Posts.Include("User").Include("Comments").Include("Comments.User").Where(pst => pst.Id == comment.PostId).First();

                SetAccessRights();

                return View(post);
            }
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
            var sanitizer = new HtmlSanitizer();

            post.Date = DateTime.Now;
            post.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                post.Content = sanitizer.Sanitize(post.Content);
              
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
            Post post = db.Posts.Where(pst => pst.Id == id).First();

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
            var sanitizer = new HtmlSanitizer();

            Post post = db.Posts.Find(id);
            
            if (ModelState.IsValid)
            {

                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    reqpost.Content = sanitizer.Sanitize(reqpost.Content);
                    //post.Date = DateTime.Now;
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
            Post post = db.Posts.Include("Comments").Where(art => art.Id == id).First();
            
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
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Moderator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
