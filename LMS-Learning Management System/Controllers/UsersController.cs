using LMS_Learning_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Controllers
{
    public class UsersController : Controller
    {
        private UserManager<AppUser> userManager;
        public UsersController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        [Authorize]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> Index()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);
            string message = "Hello " + user.UserName;
            return View((object)message);
        }
    }
}
