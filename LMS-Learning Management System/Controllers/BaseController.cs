using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace LMS_Learning_Management_System.Controllers
{
    public class BaseController : Controller
    {
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();

        public BaseController(Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden)
        {
            userManager = userMgr;
            _contextUsers = iden;

        }
       
        public void GetUserRole()
        {
            try
            {
                var userid1 = User.Identity.GetUserId();
                var username = _contextUsers.Users.Select(o => new { o.Id, o.UserName, o.Email, o.UserTypeDesc, o.FullName }).Where(m => m.Id == userid1).SingleOrDefault();
                TempData["FullName"] = username.FullName;

            }
            catch (Exception ex)
            {

            }

        }

    }
}
