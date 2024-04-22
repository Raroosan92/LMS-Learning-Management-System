using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IPasswordHasher<AppUser> passwordHasher;

        public LMSContext _context = new LMSContext();
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();

        public AdminController(Microsoft.AspNetCore.Identity.UserManager<AppUser> usrMgr, IPasswordHasher<AppUser> passwordHash, AppIdentityDbContext iden, IWebHostEnvironment hostingEnvironment)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
            _hostingEnvironment = hostingEnvironment;
            _contextUsers = iden;

        }
        [Authorize(Roles = "admin")]

        [HttpPost]
        public IActionResult GetData()
        {

            List<AppUser> stList = new List<AppUser>();

            if (User.IsInRole("admin"))
            {
                //stList = _context.Lessons.Include(l => l.Class).Include(l => l.Subject).OrderByDescending(r => r.Id).ToList();
                stList = userManager.Users.Where(R => R.Id != "8a40818b-4d93-46d7-86e0-10ae13b21932").OrderByDescending(r => r.Id).ToList();
            }

            return new JsonResult(new { data = stList });

        }

        public IActionResult Index()
        {
            return View(userManager.Users.Where(R => R.Id != "8a40818b-4d93-46d7-86e0-10ae13b21932"));
            //return View();
        }
        [AllowAnonymous]

        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");
            ViewData["RoleId"] = new SelectList(_context.AspNetRoles, "Id", "Name");
            TempData["AlertMessage"] = "";
            return View();
        }
        public List<SelectListItem> _Classes { get; set; }
        public List<SelectListItem> _Subjecs { get; set; }

        List<string> _ClassesLst = new List<string>();
        List<string> _SubjecsLst = new List<string>();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(User user, IFormFile uploadedPhoto)
        {
            if (ModelState.IsValid)
            {
                var PhotoPath = "";
              
                string email;
                string Uname;
                try
                {
                    email = userManager.Users.Where(c => c.PhoneNumber == user.PhoneNumber).FirstOrDefault().Email;

                }
                catch (Exception)
                {
                    email = null;
                }
                
                try
                {
                    Uname = userManager.Users.Where(c => c.FullName.ToLower() == user.FullName.ToLower()).FirstOrDefault().FullName;

                }
                catch (Exception)
                {
                    Uname = null;
                }

                if (Uname != null)
                {
                    ModelState.AddModelError("", "اسم المستخدم تم استخدامه مسبقاً");
                }

                if (email != null)
                {
                    ModelState.AddModelError("", "رقم الهاتف تم استخدامه مسبقاً");
                }
                else
                {
                    try
                    {
                    if (uploadedPhoto.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(uploadedPhoto.FileName);
                        var fileName = user.PhoneNumber;

                        // Generate a unique identifier for the new file name
                        //var uniqueFileName = $"{lesson_name}{lesson_subject}_WorkSheet_{w}{fileExtension}";
                        var uniqueFileName = fileName;

                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Photos", uniqueFileName + fileExtension);

                        // Check if the "ProductsImageuploads" folder exists, and create it if not
                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedPhoto.CopyToAsync(stream);
                        }

                        PhotoPath = "/Photos/" + uniqueFileName + fileExtension;

                    }

                    }
                    catch (Exception)
                    {

                        
                    }


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
                        Photo = PhotoPath,

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
                            var userid1 = _contextUsers.Users.Select(o => new { o.Id, o.Email, o.UserTypeDesc }).Where(m => m.Email == user.Email).SingleOrDefault();

                            AppUser user2 = await userManager.FindByIdAsync(userid1.Id);
                            if (user2 != null)
                            {
                                if (userid1.UserTypeDesc == "teacher")
                                {

                                    result = await userManager.AddToRoleAsync(user2, "teacher");
                                    if (!result.Succeeded)
                                        Errors(result);
                                }
                                else if (userid1.UserTypeDesc == "student")
                                {
                                    result = await userManager.AddToRoleAsync(user2, "student");
                                    if (!result.Succeeded)
                                        Errors(result);
                                }
                                else
                                {
                                    result = await userManager.AddToRoleAsync(user2, "admin");
                                    if (!result.Succeeded)
                                        Errors(result);
                                }
                            }


                            if (result.Succeeded)
                            {
                                try
                                {


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
                                catch (Exception ex)
                                {


                                }

                            }

                        }
                        TempData["AlertMessage"] = "تم انشاء الحساب بنجاح";
                        if (User.IsInRole("admin"))
                        {
                            return View();

                        }
                        else
                        {
                            return RedirectToAction("login", "Account");

                        }

                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
            }
            ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");
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



        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Update(string id)
        {
            //var Subjectes_Info = await _context.Subjects.Select(r => new { r.Id, r.Name }).Distinct().ToListAsync();
            //var Classes_Info = await _context.Classes.Select(r => new {  r.Id, r.Descriptions }).Distinct().ToListAsync();


            var query = from c in _context.Classes
                        join vt in _context.VTechersInfos on c.Id equals vt.ClassId into vtGroup
                        from vtItem in vtGroup.DefaultIfEmpty()
                        select new
                        {
                            ClassId = c.Id,
                            ClassName = c.Descriptions,
                            TeacherName = vtItem != null ? vtItem.FullName : null,
                            TeacherID = vtItem != null ? vtItem.Id : null,
                            // Include other columns as needed
                        };

            var Classes_Info = await query.Distinct().ToListAsync();



            var query2 = from s in _context.Subjects
                         join vt in _context.VTechersInfos on s.Id equals vt.SubjectId into vtGroup
                         from vtItem in vtGroup.DefaultIfEmpty()
                         select new
                         {
                             SubjectId = s.Id,
                             SubjectName = s.Name,
                             TeacherName = vtItem != null ? vtItem.FullName : null,
                             TeacherID = vtItem != null ? vtItem.Id : null,
                             // Include other columns as needed
                         };

            var Subjectes_Info = await query2.Distinct().ToListAsync();




            AppUser user = await userManager.FindByIdAsync(id);

            // Assuming you have a DbSet<AppUser> in your DbContext
            //AppUser user = await _context.AppUsers.Include(x => x.)
            //                                .FirstOrDefaultAsync(x => x.Id == id);

            List<int?> Subjectes = new List<int?>();
            List<int?> Classes = new List<int?>();

            try
            {

                Subjectes_Info.Where(r => r.TeacherID == id).Distinct().ToList().ForEach(result => Subjectes.Add(result.SubjectId));
                Classes_Info.Where(r => r.TeacherID == id).Distinct().ToList().ForEach(result => Classes.Add(result.ClassId));


            }
            catch (Exception ex)
            {


            }
            user.SelectedClasses = Classes.ToArray();
            user.SelectedSubjectes = Subjectes.ToArray();


            user.ESelectedSubjectes = Subjectes_Info.Select(r => new { r.SubjectName, r.SubjectId }).Distinct()
                .Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.SubjectName.ToString(),
                    Value = x.SubjectId.ToString()
                })
                .ToList();


            user.ESelectedClasses = Classes_Info.Select(r => new { r.ClassName, r.ClassId }).Distinct()
                .Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.ClassName.ToString(),
                    Value = x.ClassId.ToString()
                })
                .ToList();



            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Update(string id, string email, string FullName, string PhoneNumber, string password, List<string> SelectedClasses, List<string> SelectedSubjectes, IFormFile uploadedPhoto)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(FullName))
                    user.FullName = FullName;
                else
                    ModelState.AddModelError("", "FullName cannot be empty");

                if (!string.IsNullOrEmpty(PhoneNumber))
                    user.PhoneNumber = PhoneNumber;
                else
                    ModelState.AddModelError("", "PhoneNumber cannot be empty");

                //if (!string.IsNullOrEmpty(email))
                //    user.Email = email;
                //else
                //    ModelState.AddModelError("", "Email cannot be empty");


                //if (!string.IsNullOrEmpty(password))
                //    user.PasswordHash = passwordHasher.HashPassword(user, password);
                //else
                //    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = passwordHasher.HashPassword(user, password);


                //if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                //{
                var PhotoPath = "";
                if (uploadedPhoto.Length > 0)
                {
                    var fileExtension = Path.GetExtension(uploadedPhoto.FileName);
                    var fileName = user.PhoneNumber;

                    // Generate a unique identifier for the new file name
                    //var uniqueFileName = $"{lesson_name}{lesson_subject}_WorkSheet_{w}{fileExtension}";
                    var uniqueFileName = fileName;

                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Photos", uniqueFileName+fileExtension);

                    // Check if the "ProductsImageuploads" folder exists, and create it if not
                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedPhoto.CopyToAsync(stream);
                    }

                    PhotoPath = "/Photos/" + uniqueFileName+ fileExtension;

                }
                user.Photo = PhotoPath;

                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    //*************************for Subjects and Classes**************************
                    //var teacherEnrollments = _context.TeacherEnrollments
                    //    .Where(te => te.UserId == user.Id)
                    //    .ToList();

                    //foreach (var enrollment in teacherEnrollments)
                    //{
                    //    enrollment.SelectedClasses = SelectedClasses;
                    //    enrollment.SelectedSubjectes = SelectedSubjectes;
                    //}

                   


                    try
                    {


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

                        //string[] ClassId = Request.Form["lstClasses"].ToString().Split(",");
                        //string[] SubjecsId = Request.Form["lstSubjecs"].ToString().Split(",");



                        foreach (string id2 in SelectedClasses)
                        {
                            if (!string.IsNullOrEmpty(id2))
                            {
                                string name = _Classes.Where(x => x.Value == id2).FirstOrDefault().Text;
                                _ClassesLst.Add(id2);
                            }
                        }

                        foreach (string id2 in SelectedSubjectes)
                        {
                            if (!string.IsNullOrEmpty(id2))
                            {
                                string name = _Classes.Where(x => x.Value == id2).FirstOrDefault().Text;
                                _SubjecsLst.Add(id2);
                            }
                        }




                        var cc = _context.TeacherEnrollments.Where(x => x.UserId == id).ToList();

                        _context.TeacherEnrollments.RemoveRange(cc);
                        await _context.SaveChangesAsync();


                        var TeacherEnrollment = new TeacherEnrollment();


                        for (int a = 0; a < _ClassesLst.Count; a++)
                        {
                            for (int b = 0; b < _SubjecsLst.Count; b++)
                            {
                                TeacherEnrollment = new TeacherEnrollment
                                {
                                    UserId = id,
                                    SubjectId = int.Parse(_SubjecsLst[b]),
                                    ClassId = int.Parse(_ClassesLst[a])
                                };
                                _context.TeacherEnrollments.Add(TeacherEnrollment);
                                await _context.SaveChangesAsync();
                            }
                        }

                    }
                    catch (Exception ex)
                    {


                    }



                    //*************************for Subjects and Classes**************************

                    return RedirectToAction("Index");
                }

                //}
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            // If there are errors or the user is not found, return to the view with the current user data
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
                if (!await userManager.IsInRoleAsync(user, "admin"))
                {
                    IdentityResult result = await userManager.DeleteAsync(user);
                    if (result.Succeeded)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false, message = "فشل حذف المستخدم." });
                }
                else
                {
                    return Json(new { success = false, message = "لا يمكن حذف المستخدمين الادمن." });
                }
            }
            else
            {
                return Json(new { success = false, message = "لم يتم العثور على المستخدم." });
            }
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
