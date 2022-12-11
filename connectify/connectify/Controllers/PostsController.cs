using connectify.Data;
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
            

            return View();
        }
    }
}
