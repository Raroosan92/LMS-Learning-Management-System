using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize]

    public class AdminController : Controller
    {
        string time;
        string Year;
        string Month;
        DateTime Jor;
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        private IPasswordHasher<AppUser> passwordHasher;

        public LMSContext _context = new LMSContext();
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();

        public AdminController(Microsoft.AspNetCore.Identity.UserManager<AppUser> usrMgr, IPasswordHasher<AppUser> passwordHash, AppIdentityDbContext iden)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
            _contextUsers = iden;

        }

        public IActionResult Index()
        {
            return View(userManager.Users);
        }
        [AllowAnonymous]

        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation");
            ViewData["RoleId"] = new SelectList(_context.AspNetRoles, "Id", "Name");
            return View();
        }
        public List<SelectListItem> _Classes { get; set; }
        public List<SelectListItem> _Subjecs { get; set; }

        List<string> _ClassesLst = new List<string>();
        List<string> _SubjecsLst = new List<string>();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                GetTime();
                string UserTypeDesc;
                string UserType;
                if (GetUserRole().ToUpper() != "ADMIN")
                {
                    UserTypeDesc = "student";
                    UserType = "7c72ca3d-4714-4340-b0d0-99cc56ef6623";
                }
                else
                {
                    UserTypeDesc = user.UserTypeDesc;
                    UserType = user.RoleId;
                }
                AppUser appUser = new AppUser
                {
                    UserName = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CreatedDateTime = Jor,
                    FullName = user.FullName,
                    Country = user.Country,

                    UserTypeDesc = UserTypeDesc,
                    UserType = UserType,
                    BirthDate = user.BirthDate


                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);



                if (result.Succeeded)
                {
                    if (GetUserRole().ToUpper() != "ADMIN")
                    {

                        RoleModification model = new RoleModification();
                        var userid1 = _contextUsers.Users.Select(o => new { o.Id, o.Email }).Where(m => m.Email == user.Email).SingleOrDefault();

                        AppUser user2 = await userManager.FindByIdAsync(userid1.Id);
                        if (user2 != null)
                        {
                            result = await userManager.AddToRoleAsync(user2, "student");
                            if (!result.Succeeded)
                                Errors(result);

                        }
                    }
                    else
                    {


                        RoleModification model = new RoleModification();
                        var userid1 = _contextUsers.Users.Select(o => new { o.Id, o.Email }).Where(m => m.Email == user.Email).SingleOrDefault();

                        AppUser user2 = await userManager.FindByIdAsync(userid1.Id);
                        if (user2 != null)
                        {
                            result = await userManager.AddToRoleAsync(user2, "teacher");
                            if (!result.Succeeded)
                                Errors(result);

                        }


                        _Classes = (from Classes1 in _context.Classes
                                    select new SelectListItem
                                    {
                                        Text = Classes1.Descriptions,
                                        Value = Classes1.Id.ToString()
                                    }).ToList();



                        _Subjecs = (from Subjects1 in _context.Subjects
                                    select new SelectListItem
                                    {
                                        Text = Subjects1.Name,
                                        Value = Subjects1.Id.ToString()
                                    }).ToList();


                        _Classes = _Classes.OrderBy(v => v.Value).Distinct().ToList();
                        _Subjecs = _Subjecs.OrderBy(v => v.Value).Distinct().ToList();

                        string[] ClassId = Request.Form["lstClasses"].ToString().Split(",");
                        string[] SubjecsId = Request.Form["lstSubjecs"].ToString().Split(",");



                        foreach (string id in ClassId)
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                string name = _Classes.Where(x => x.Value == id).FirstOrDefault().Text;
                                _ClassesLst.Add(id);
                            }
                        }

                        foreach (string id2 in SubjecsId)
                        {
                            if (!string.IsNullOrEmpty(id2))
                            {
                                string name = _Classes.Where(x => x.Value == id2).FirstOrDefault().Text;
                                _SubjecsLst.Add(id2);
                            }
                        }
                        var TeacherEnrollment = new TeacherEnrollment();


                        for (int a = 0; a < _ClassesLst.Count; a++)
                        {
                            for (int b = 0; b < _SubjecsLst.Count; b++)
                            {
                                TeacherEnrollment = new TeacherEnrollment
                                {
                                    UserId = userid1.Id,
                                    SubjectId = int.Parse(_SubjecsLst[b]),
                                    ClassId = int.Parse(_ClassesLst[a])
                                };
                                _context.TeacherEnrollments.Add(TeacherEnrollment);
                                await _context.SaveChangesAsync();
                            }
                        }




                    }
                    return RedirectToAction("Index");

                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation");
            ViewData["RoleId"] = new SelectList(_context.AspNetRoles, "Id", "Name");
            return View(user);
        }
        public void GetTime()
        {

            TimeZoneInfo AST = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");
            DateTime utc = DateTime.UtcNow;
            Jor = TimeZoneInfo.ConvertTimeFromUtc(utc, AST);
            time = Jor.ToString();
            Year = Jor.Year.ToString();
            Month = Jor.Month.ToString();

        }
        public async Task<IActionResult> Update(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, string email, string password)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }


        public string GetUserRole()
        {
            try
            {
                var userid1 = User.Identity.GetUserId();
                var username = _contextUsers.Users.Select(o => new { o.Id, o.UserName, o.Email, o.UserTypeDesc }).Where(m => m.Id == userid1).SingleOrDefault();
                return username.UserTypeDesc.ToString();

            }
            catch (Exception ex)
            {

                return "null";
            }

        }
    }
}
