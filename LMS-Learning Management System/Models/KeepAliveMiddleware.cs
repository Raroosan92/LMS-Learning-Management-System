using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Models
{
    public class KeepAliveMiddleware
    {
         
            private readonly RequestDelegate _next;

            public KeepAliveMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var sessionId = context.User.FindFirst("SessionId")?.Value;

                    using (var dbContext = new LMSContext())
                    {
                        var userSession = dbContext.ActiveSessions.SingleOrDefault(u => u.UserId == userId);
                        if (userSession == null || userSession.MacAddress != sessionId)
                        {
                            // Invalidate the current session and log out the user
                            await context.SignOutAsync();
                            context.Response.Redirect("/Account/Login");
                            return;
                        }
                    }
                }
                await _next(context);
            }
         

    }
}
