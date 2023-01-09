using connectify.Data;
using connectify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace connectify.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public MessagesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Delete(int id)
        {
            Message msg= db.Messages.Find(id);
            
            if (msg.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Messages.Remove(msg);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + msg.GroupId);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti mesajul";
                return RedirectToAction("Index", "Groups");
            }
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id)
        {
            Message msg = db.Messages.Find(id);

            if (msg.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(msg);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati mesajul";
                return RedirectToAction("Index", "Groups");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id, Message requestMessage)
        {
            Message msg = db.Messages.Find(id);

            if (msg.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    msg.Content = requestMessage.Content;

                    db.SaveChanges();

                    return Redirect("/Groups/Show/" + msg.GroupId);
                }
                else
                {
                    return View(requestMessage);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Groups");
            }
        }
    }
}
