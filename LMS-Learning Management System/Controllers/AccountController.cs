using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private readonly LMSContext _context;
        public AccountController(Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr, LMSContext context)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login();
            login.ReturnUrl = returnUrl;
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

                var userdetails = userManager.Users.Where(c => c.PhoneNumber == login.PhoneNumber).FirstOrDefault();
                AppUser appUser = await userManager.FindByEmailAsync(userdetails.Email.ToString());
                if (GetActiveSession(userdetails.Id))
                {

                    if (appUser != null)
                    {
                        await signInManager.SignOutAsync();
                        Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, login.Remember,false);

                        if (result.Succeeded)
                        {
                            GetTime();

                            //activesession.LoginDate = DateTime.Parse(time);
                            //activesession.UserId = appUser.Id;
                            //activesession.UserName = appUser.FullName;
                            //activesession.PhoneNumber = appUser.PhoneNumber;
                            //activesession.DeviceType = login.devicetype;
                            //_context.Add(activesession);

                            //await _context.SaveChangesAsync();
                            TempData["FullName"] = appUser.FullName;

                            //// In your controller or middleware
                            //HttpContext.Session.SetString("TestKey", "TestValue");

                            //// Check if the value is present in subsequent requests
                            //var testValue = HttpContext.Session.GetString("TestKey");


                            return Redirect(login.ReturnUrl ?? "/");
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
                    TempData["AlertMessage"] = "تم تسجيل الدخول من جهاز آخر. قم بعملية تسجل الخروج منه ثم تسجيل الدخول هنا مرة اخرى";

                }

                }
                catch (Exception ex)
                {

                    TempData["AlertMessage"] = "يوجد خطأ في عملية تسجيل الدخول يرجى التأكد من اسم المستخدم وكلمة المرور اولاً";

                }
            }
            return View(login);
        }

        public bool GetActiveSession(string userId)
        {
            try
            {
                var username = _context.ActiveSessions.Where(m => m.UserId == userId).SingleOrDefault();
                if (username != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {

                return false;
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
            if (await DeleteActiveSession())
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Login", "Account");

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
