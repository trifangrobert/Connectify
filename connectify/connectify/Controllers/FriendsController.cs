using connectify.Data;
using connectify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace connectify.Controllers
{
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;


        public FriendsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            )
        {
            db = context;

            _userManager = userManager;
        }
        // get all friends of the current user
        public IActionResult Index()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var friends = db.Friends.Where(f => f.User == currentUser && f.Status == "Accepted").Select(f => f.UserFriend).ToList();
            var aux = db.Friends.Where(f => f.UserFriend == currentUser && f.Status == "Accepted").Select(f => f.User).ToList();
            friends.AddRange(aux);
            ViewBag.friends = friends;
            return View();
        }


        //get all friend requests that current user have
        public IActionResult Requests()
        {
            var userId = _userManager.GetUserId(User);
            var friendRequests = db.Friends.Where(f => f.UserFriendId == userId && f.Status == "Pending").ToList();
            ApplicationUser[] l = new ApplicationUser[friendRequests.Count];
            for (int i = 0; i < friendRequests.Count; i++)
            {
                var currId = friendRequests[i].UserId;
                l[i] = db.Users.Where(u => u.Id == currId).FirstOrDefault();
            }
            
            ViewBag.friendRequests = l;
            return View();
        }

        //accept friend request
        public IActionResult Accept(string id)
        {
            var userId = _userManager.GetUserId(User);
            var friendRequest = db.Friends.Where(f => f.UserId == id && f.UserFriendId == userId).FirstOrDefault();
            friendRequest.Status = "Accepted";

            db.SaveChanges();
            
            return RedirectToAction("Requests");
        }

        //reject friend request

        public IActionResult Reject(string id)
        {
            var userId = _userManager.GetUserId(User);
            var friendRequest = db.Friends.Where(f => f.UserId == id && f.UserFriendId == userId).FirstOrDefault();
            friendRequest.Status = "Rejected";
            db.SaveChanges();
            return RedirectToAction("Requests");
        }
    }
}
