using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Models
{
    public class SecurityStampMiddleware
    {
       
            private readonly RequestDelegate _next;
            private readonly UserManager<AppUser> _userManager;

            public SecurityStampMiddleware(RequestDelegate next, UserManager<AppUser> userManager)
            {
                _next = next;
                _userManager = userManager;
            }

            public async Task Invoke(HttpContext context)
            {
                var currentUser = await _userManager.GetUserAsync(context.User);

                if (currentUser != null && currentUser.SecurityStamp != context.User.FindFirstValue("AspNet.Identity.SecurityStamp"))
                {
                    // تغيرت علامة الأمان، ربما تم تسجيل الدخول من مكان آخر
                    // يمكنك اتخاذ الإجراءات المناسبة هنا
                }

                await _next(context);
            }
        


    }
}
