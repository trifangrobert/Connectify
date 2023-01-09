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
            SetGroupRights(group);
            // get owner of the group
            var owner = db.Users.Where(u => u.Id == group.OwnerId).First();
            ViewBag.Owner = owner;
            return View(group);
        }


        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show([FromForm] Message message)
        {
            message.Date = DateTime.Now;
            message.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + message.GroupId);
            }

            else
            {
                Group group = db.Groups.Include("Users").Include("Messages").Include("Messages.User").Where(g => g.Id == message.GroupId).First();

                SetAccessRights();
                SetGroupRights(group);
                return View(group);
            }
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
        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Join(int id)
        {
            Group group = db.Groups.Where(g => g.Id == id).First();
            ApplicationUser user = db.Users.Where(u => u.Id == _userManager.GetUserId(User)).First();
            ApplicationUserGroup userGroup = new ApplicationUserGroup() { ApplicationUser = user, Group = group };
            
            if (db.ApplicationUserGroups.Where(ug => ug.GroupId == group.Id && ug.ApplicationUserId == user.Id) == null || db.ApplicationUserGroups.Where(ug => ug.GroupId == group.Id && ug.ApplicationUserId == user.Id).Count() == 0)
            {
                db.ApplicationUserGroups.Add(userGroup);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "You are already a member of this group!";
            }
            SetGroupRights(group);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Leave(int id)
        {
            Group group = db.Groups.Where(g => g.Id == id).First();
            ApplicationUser user = db.Users.Where(u => u.Id == _userManager.GetUserId(User)).First();

            var ug = db.ApplicationUserGroups.Where(ug => ug.GroupId == group.Id && ug.ApplicationUserId == user.Id).FirstOrDefault();
            if (ug == null || ug.GroupId != group.Id || ug.ApplicationUserId != user.Id)
            {
                TempData["message"] = "You are not a member of this group!";
            }
            else
            {
                db.ApplicationUserGroups.Remove(ug);
                db.SaveChanges();
            }
            SetGroupRights(group);
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

        [NonAction]
        private void SetGroupRights(Group group)
        {
            ViewBag.IsOwner = false;
            ViewBag.IsMember = false;

            if (group.OwnerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                ViewBag.IsOwner = true;
            }
            else if (db.ApplicationUserGroups.Where(ug => ug.GroupId == group.Id && ug.ApplicationUserId == _userManager.GetUserId(User)).Count() > 0)
            {
                ViewBag.IsMember = true;
            }
        }

    }
}