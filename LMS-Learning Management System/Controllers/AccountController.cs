using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private readonly LMSContext _context; 
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr, LMSContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }
        [AllowAnonymous]
        public string GetAppVersion()
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            //SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            cmd.CommandText = "SELECT top 1 App_Version FROM App_Version;";
            connection.Open();
            var xwpf = cmd.ExecuteScalar();
            connection.Close();
            return xwpf.ToString();
        }
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login();
            login.ReturnUrl = returnUrl;





            //string wpfAppIdentifier2 = HttpContext.Request.Headers["X-WPF-App-Identifier"];

            //// Store the value in the session
            //if (wpfAppIdentifier2 != null)
            //{
            //    HttpContext.Session.SetString("X-WPF-App-Identifier", wpfAppIdentifier2);

            //}

            //string cookieValueFromWpf = HttpContext.Request.Cookies["X-WPF-App-Identifier"];
            //if (cookieValueFromWpf != null)
            //{
            //    Response.Cookies.Append("X-WPF-App-Identifier", cookieValueFromWpf, new CookieOptions { Expires = DateTime.Now.AddHours(3) });

            //}
            return View(login);
        }
        string time;
        string Year;
        string Month;
        DateTime Jor;
        public void GetTime()
        {

            TimeZoneInfo AST = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");
            DateTime utc = DateTime.UtcNow;
            Jor = TimeZoneInfo.ConvertTimeFromUtc(utc, AST);
            time = Jor.ToString();
            Year = Jor.Year.ToString();
            Month = Jor.Month.ToString();

        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login, ActiveSession activesession)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //string wpfAppIdentifier2 = HttpContext.Request.Headers["X-WPF-App-Identifier"];

                    //// Store the value in the session
                    //if (wpfAppIdentifier2 != null)
                    //{
                    //    HttpContext.Session.SetString("X-WPF-App-Identifier", wpfAppIdentifier2);

                    //}
                    //string cookieValueFromWpf = HttpContext.Request.Cookies["X-WPF-App-Identifier"];
                    //if (cookieValueFromWpf!=null)
                    //{
                    //    Response.Cookies.Append("X-WPF-App-Identifier", cookieValueFromWpf, new CookieOptions { Expires = DateTime.Now.AddHours(3) });

                    //}
                    string ActiveSessions = "";
                    var userdetails = userManager.Users.Where(c => c.PhoneNumber == login.PhoneNumber).FirstOrDefault();
                    AppUser appUser = await userManager.FindByEmailAsync(userdetails.Email.ToString());
                    var roles = await userManager.GetRolesAsync(appUser);

                    if (roles[0].ToLower()=="admin")
                    {
                        ActiveSessions = "Succeeded";

                    }
                    else
                    {

                        ActiveSessions = GetActiveSession(userdetails.Id,login.clientIpAddress);
                    }

                    if (ActiveSessions == "Succeeded" || ActiveSessions == "NotExist")
                    {
                        string macAddressString = "";
                        if (appUser != null)
                        {
                            await signInManager.SignOutAsync();
                            Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);

                            if (result.Succeeded)
                            {
                                if (ActiveSessions == "NotExist")
                                {
                                    string computerName = Environment.MachineName;

                                    //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                                    //{
                                    //    if (nic.OperationalStatus == OperationalStatus.Up && !nic.Description.ToLowerInvariant().Contains("virtual"))
                                    //    {
                                    //        PhysicalAddress macAddress = nic.GetPhysicalAddress();
                                    //        macAddressString = BitConverter.ToString(macAddress.GetAddressBytes());
                                    //        // Now, macAddressString contains the MAC address of the first active non-virtual network interface.
                                    //        break;
                                    //    }
                                    //}


                                    GetTime();
                                    activesession.LoginDate = DateTime.Parse(time);
                                    activesession.UserId = appUser.Id;
                                    activesession.UserName = appUser.FullName;
                                    activesession.PhoneNumber = appUser.PhoneNumber;
                                    activesession.DeviceType = login.devicetype;
                                    activesession.ComputerName = computerName;
                                    activesession.MacAddress = login.clientIpAddress;
                                    //activesession.MacAddress = macAddressString;
                                    _context.Add(activesession);

                                    await _context.SaveChangesAsync();

                                    //// In your controller or middleware
                                    //HttpContext.Session.SetString("TestKey", "TestValue");

                                    //// Check if the value is present in subsequent requests
                                    //var testValue = HttpContext.Session.GetString("TestKey");
                                }
                                TempData["FullName"] = appUser.FullName;

                                //return Redirect(login.ReturnUrl ?? "/");
                                return Redirect(login.ReturnUrl ?? "/Academy");
                            }

                            // uncomment Two Factor Authentication https://www.yogihosting.com/aspnet-core-identity-two-factor-authentication/
                            /*if (result.RequiresTwoFactor)
                            {
                                return RedirectToAction("LoginTwoStep", new { appUser.Email, login.ReturnUrl });
                            }*/

                            // Uncomment Email confirmation https://www.yogihosting.com/aspnet-core-identity-email-confirmation/
                            /*bool emailStatus = await userManager.IsEmailConfirmedAsync(appUser);
                            if (emailStatus == false)
                            {
                                ModelState.AddModelError(nameof(login.Email), "Email is unconfirmed, please confirm it first");
                            }*/

                            // Uncomment user lockout https://www.yogihosting.com/aspnet-core-identity-user-lockout/
                            /*if (result.IsLockedOut)
                                ModelState.AddModelError("", "Your account is locked out. Kindly wait for 10 minutes and try again");*/
                        }
                        ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
                    }
                    else
                    {
                        TempData["AlertMessage"] = "يوجد جهاز اخر قام بعملية تسجيل الدخول, يرجى التواصل مع ادارة المنصة لحل المشكلة";

                    }

                }
                catch (Exception ex)
                {

                    TempData["AlertMessage"] = "يوجد خطأ في عملية تسجيل الدخول يرجى التأكد من اسم المستخدم وكلمة المرور اولاً";

                }
            }
            return View(login);
        }







        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await userManager.FindByNameAsync(model.Username);
        //        if (user != null && await signInManager.CheckPasswordSignInAsync(user, model.Password, false) == SignInResult.Success)
        //        {
        //            // Check if user has an active session
        //            var existingSession = await sessionService.GetActiveSessionAsync(user.Id);
        //            if (existingSession != null)
        //            {
        //                // Compare device identifier with the current device
        //                if (existingSession.DeviceId != model.DeviceId)
        //                {
        //                    ModelState.AddModelError(string.Empty, "You are already logged in from another device.");
        //                    return View(model);
        //                }
        //            }

        //            // Store session identifier and device identifier
        //            await sessionService.CreateSessionAsync(user.Id, model.DeviceId);

        //            // Redirect to the dashboard or home page
        //            return RedirectToAction("Dashboard", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid username or password.");
        //        }
        //    }

        //    return View(model);
        //}










        //public static string GetClientIPAddress(HttpContext context)
        //{
        //    string ipAddress = context.Connection.LocalIpAddress?.ToString();
        //    return ipAddress;
        //}

        public static string GetUniqueMachineId()
        {
            try
            {
                var macAddresses22 = NetworkInterface.GetAllNetworkInterfaces();
                // Retrieve MAC addresses of all network interfaces
                var macAddresses = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.OperationalStatus == OperationalStatus.Up)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .ToList();

                // Concatenate MAC addresses and compute a hash
                using (var sha256 = SHA256.Create())
                {
                    var concatenatedAddresses = string.Join("", macAddresses.OrderBy(addr => addr));
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedAddresses));

                    // Convert hash to a hexadecimal string
                    var machineId = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                    return machineId;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public string GetActiveSession(string userId, string clientIpAddress)
        {
            try
            {
                string macAddressString = "";


                var username = _context.ActiveSessions.Where(m => m.UserId == userId).SingleOrDefault();

                if (username != null)
                {
                    string computerName = Environment.MachineName;

                    //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    //{
                    //    if (nic.OperationalStatus == OperationalStatus.Up && !nic.Description.ToLowerInvariant().Contains("virtual"))
                    //    {
                    //        PhysicalAddress macAddress = nic.GetPhysicalAddress();
                    //        macAddressString = BitConverter.ToString(macAddress.GetAddressBytes());
                    //        // Now, macAddressString contains the MAC address of the first active non-virtual network interface.
                    //        break;
                    //    }
                    //}
                    macAddressString = clientIpAddress;

                    if (macAddressString == username.MacAddress)
                    {
                        return "Succeeded";
                    }
                    else
                    {
                        return "Faild";
                    }
                }
                else
                {
                    return "NotExist";
                }

            }
            catch (Exception ex)
            {

                return "Faild";
            }

        }

        public async Task<bool> DeleteActiveSession()

        {
            return true;
            //try
            //{
            //    ActiveSession activesession = new ActiveSession();
            //    var userid1 =   User.Identity.GetUserId();
            //    var username =  _context.ActiveSessions.Where(m => m.UserId == userid1).SingleOrDefault();

            //    var session =  _context.ActiveSessions.Find(username.Id);

            //    if (session != null)
            //    {
            //         _context.ActiveSessions.Remove(session);
            //        await _context.SaveChangesAsync();
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }

            //}
            //catch (Exception ex)
            //{

            //    return true;
            //}

        }
        public async Task<IActionResult> Logout()
        {
            TempData["AlertMessage"] = "";
            if (await DeleteActiveSession())
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");

            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "admin")]

        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };
            if (result.Succeeded)
                return View(userInfo);
            else
            {
                AppUser user = new AppUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
                };

                Microsoft.AspNetCore.Identity.IdentityResult identResult = await userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                        return View(userInfo);
                    }
                }
                return AccessDenied();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginTwoStep(string email, string returnUrl)
        {
            var user = await userManager.FindByEmailAsync(email);

            var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmailTwoFactorCode(user.Email, token);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginTwoStep(TwoFactor twoFactor, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(twoFactor.TwoFactorCode);
            }

            var result = await signInManager.TwoFactorSignInAsync("Email", twoFactor.TwoFactorCode, false, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Attempt");
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            if (!ModelState.IsValid)
                return View(email);

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmailPasswordReset(user.Email, link);

            if (emailResponse)
                return RedirectToAction("ForgotPasswordConfirmation");
            else
            {
                // log email failed 
            }
            return View(email);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (!ModelState.IsValid)
                return View(resetPassword);

            var user = await userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
                RedirectToAction("ResetPasswordConfirmation");

            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return View();
            }

            return RedirectToAction("ResetPasswordConfirmation");
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
