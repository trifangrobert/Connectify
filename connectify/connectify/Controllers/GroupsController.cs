using connectify.Data;
using connectify.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace connectify.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index()
        {
            var groups = db.Groups.ToList();

            ViewBag.Groups = groups;

            return View();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            Group group = db.Groups.Include("Messages").Include("Messages.User").Where(g => g.Id == id).First();
            SetAccessRights();
            return View(group);
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New()
        {
            Group group = new Group();
            return View(group);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New(Group group)
        {
            var sanitizer = new HtmlSanitizer();

            group.OwnerId = _userManager.GetUserId(User);
            Console.WriteLine(group.OwnerId);
            if (ModelState.IsValid)
            {
                group.Description = sanitizer.Sanitize(group.Description);

                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(group);
            }

        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id)
        {
            Group group = db.Groups.Where(g => g.Id == id).First();
            if (group.OwnerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(group);
            }
            else
            {
                TempData["message"] = "You are not authorized to edit this group!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id, Group reqgroup)
        {
            Group group = db.Groups.Find(id);
            
            if (ModelState.IsValid)
            {
                if (group.OwnerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    group.Name = reqgroup.Name;
                    group.Description = reqgroup.Description;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "You are not authorized to edit this group!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(group);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Delete(int id)
        {
            Group group = db.Groups.Include("Messages").Where(g => g.Id == id).First();

            if (ModelState.IsValid)
            {
                if (group.OwnerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.Groups.Remove(group);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "You are not authorized to delete this group!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(group);
            }
        }

        public IActionResult Join(int id)
        {
            Group group = db.Groups.Where(g => g.Id == id).First();
            ApplicationUser user = db.Users.Where(u => u.Id == _userManager.GetUserId(User)).First();
            ApplicationUserGroup userGroup = new ApplicationUserGroup() { Group = group, ApplicationUser = user };
            if (db.ApplicationUserGroups.Where(ug => ug.GroupId == group.Id && ug.ApplicationUserId == user.Id) == null)
            {
                db.ApplicationUserGroups.Add(userGroup);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "You are already a member of this group!";
            }
            return RedirectToAction("Index");
        }

        [NonAction]
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
