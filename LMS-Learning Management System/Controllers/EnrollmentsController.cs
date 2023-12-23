using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize]
    public class EnrollmentsController : Controller
    {
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        private readonly LMSContext _context;
        string time;
        string Year;
        string Month;
        DateTime Jor;
        public EnrollmentsController(LMSContext context, Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden)
        {
            _context = context;
            _contextUsers = iden;
            // BaseController aa = new BaseController(userMgr,iden);
            //aa.GetUserRole();


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

        [Authorize(Roles = "admin")]
        // GET: Enrollments
        public async Task<IActionResult> Index()
        {
            GetUserRole();
            return View();
            //var lMSContext = _context.Enrollments.Include(e => e.Class).Include(e => e.Subject).Include(e => e.User);
            //return View(await lMSContext.ToListAsync());
        }

    [Authorize(Roles = "admin")]
        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Class)
                .Include(e => e.Subject)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);


            var subjects = _context.Subjects.Where(r => r.Id == enrollment.SubjectId).SingleOrDefault();
            var classes = _context.Classes.Where(r => r.Id == enrollment.ClassId).SingleOrDefault();
            var users = _context.AspNetUsers.Where(r => r.Id == enrollment.UserId).SingleOrDefault();
            enrollment.Classdesc = classes.Descriptions;
            enrollment.Subjectdesc = subjects.Name;
            enrollment.Userdesc = users.UserName;


            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

    [Authorize(Roles = "admin")]
        // GET: Enrollments/Create
        public IActionResult Create()
        {
            List<Subject> Subject_List = new List<Subject>();
            List<Class> Class_List = new List<Class>();

            if (User.IsInRole("admin"))
            {
                ViewData["ClassId"] = new SelectList(_context.Classes.Where(r=>r.Status == true), "Id", "Descriptions");
                ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");

            }
            else
            {
                var teacher_subjects = _context.TeacherEnrollments.Select(r => new { r.SubjectId, r.UserId }).Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_subjects.Count; i++)
                {
                    var x = _context.Subjects.Where(r => r.Id == teacher_subjects[i].SubjectId && r.Status==true).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (x != null)
                    {
                        Subject_List.Add(x);
                    }

                }

                var teacher_Classes = _context.TeacherEnrollments.Select(r => new { r.ClassId, r.UserId }).Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_Classes.Count; i++)
                {
                    var y = _context.Classes.Where(r => r.Id == teacher_Classes[i].ClassId && r.Status == true).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (y != null)
                    {
                        Class_List.Add(y);
                    }

                }
                ViewData["SubjectId"] = new SelectList(Subject_List, "Id", "Abbreviation");
                ViewData["ClassId"] = new SelectList(Class_List, "Id", "Descriptions");
            }

            ViewData["UserId"] = new SelectList(_context.AspNetUsers.Where(r=>r.UserType== "7c72ca3d-4714-4340-b0d0-99cc56ef6623"), "Id", "UserName");
            GetUserRole(); 
            return View();
        }
        [Authorize(Roles = "admin")]

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassId,SubjectId,UserId,CreatedDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                GetTime();
                enrollment.CreatedDate = DateTime.Parse(time);
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", enrollment.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", enrollment.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", enrollment.UserId);
            GetUserRole(); 
            return View(enrollment);
        }

    [Authorize(Roles = "admin")]
        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            List<Subject> Subject_List = new List<Subject>();
            List<Class> Class_List = new List<Class>();

            if (User.IsInRole("admin"))
            {
                ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions");
                ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation");

            }
            else
            {
                var teacher_subjects = _context.TeacherEnrollments.Select(r => new { r.SubjectId, r.UserId }).Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_subjects.Count; i++)
                {
                    var x = _context.Subjects.Where(r => r.Id == teacher_subjects[i].SubjectId).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (x != null)
                    {
                        Subject_List.Add(x);
                    }

                }

                var teacher_Classes = _context.TeacherEnrollments.Select(r => new { r.ClassId, r.UserId }).Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_Classes.Count; i++)
                {
                    var y = _context.Classes.Where(r => r.Id == teacher_Classes[i].ClassId).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (y != null)
                    {
                        Class_List.Add(y);
                    }

                }
                ViewData["SubjectId"] = new SelectList(Subject_List, "Id", "Abbreviation");
                ViewData["ClassId"] = new SelectList(Class_List, "Id", "Descriptions");
            }

            ViewData["UserId"] = new SelectList(_context.AspNetUsers.Where(r => r.UserType == "7c72ca3d-4714-4340-b0d0-99cc56ef6623"), "Id", "UserName");
            return View(enrollment);
        }
        [Authorize(Roles = "admin")]

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassId,SubjectId,UserId")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    _context.Entry(enrollment).State = EntityState.Modified;
                    _context.Entry(enrollment).Property(x => x.CreatedDate).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", enrollment.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", enrollment.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", enrollment.UserId);
            return View(enrollment);
        }

        //// GET: Enrollments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var enrollment = await _context.Enrollments
        //        .Include(e => e.Class)
        //        .Include(e => e.Subject)
        //        .Include(e => e.User)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (enrollment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(enrollment);
        //}

        //// POST: Enrollments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var enrollment = await _context.Enrollments.FindAsync(id);
        //    _context.Enrollments.Remove(enrollment);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult GetData()
        {
            List<Enrollment> stList = new List<Enrollment>();

            if (User.IsInRole("admin"))
            {
                 stList = _context.Enrollments.Include(e => e.Class).Include(e => e.Subject).Include(e => e.User).OrderByDescending(r => r.Id).ToList();

            }
            else
            {
                var teacher_Lessons = _context.TeacherEnrollments.Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_Lessons.Count; i++)
                {
                    var x = _context.Enrollments.Where(r => r.SubjectId == teacher_Lessons[i].SubjectId && r.ClassId == teacher_Lessons[i].ClassId).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (x != null)
                    {
                        stList.Add(x);
                    }

                }
            }




            for (int i = 0; i < stList.Count; i++)
            {
                var subjects = _context.Subjects.Where(r => r.Id == stList[i].SubjectId).SingleOrDefault();
                var classes = _context.Classes.Where(r => r.Id == stList[i].ClassId).SingleOrDefault();
                var users = _context.AspNetUsers.Where(r => r.Id == stList[i].UserId).SingleOrDefault();
                stList[i].Classdesc = classes.Descriptions;
                stList[i].Subjectdesc = subjects.Name;
                stList[i].Userdesc = users.UserName;

            }
            return new JsonResult(new { data = stList });

        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (LMSContext db = new LMSContext())
            {
                Enrollment std = db.Enrollments.Where(x => x.Id == id).FirstOrDefault<Enrollment>();
                db.Enrollments.Remove(std);
                db.SaveChanges();
                return Json(new { success = true, message = "تمت عملية الحذف بنجاح" });
            }
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

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }


        [Authorize(Roles = "student,admin")]

        [HttpGet]
        // GET: Enrollments/Create
        public IActionResult EnrollSTD()
        {
            GetUserRole();
            return View();
        }



        [Authorize(Roles = "student,admin")]

        public IActionResult EnrollSTD1(string Card_No, string Password_Card)
        {
            Enrollment enrollment2 = new Enrollment();
            if (ModelState.IsValid)
            {
                GetTime();
                var Card_Details = _context.Cards.Where(r => r.CardNo == int.Parse(Card_No) && r.CardPassword == Password_Card).FirstOrDefault();
                if (Card_Details.CardNo > 0)
                {

                    var card_Enrollments = _context.CardSubjects.Where(r => r.CardNo == Card_Details.Id).ToList();
                    foreach (var item in card_Enrollments)
                    {
                        var Check_EnrollmentExists = _context.Enrollments.Any(r => r.UserId == User.Identity.GetUserId() && r.SubjectId == item.SubjectId && r.ClassId == item.ClassId);
                        if (Check_EnrollmentExists == false)
                        {

                            enrollment2 = new Enrollment
                            {
                                ClassId = item.ClassId,
                                SubjectId = item.SubjectId,
                                UserId = User.Identity.GetUserId(),
                                CreatedDate = DateTime.Parse(time)
                            };
                            _context.Add(enrollment2);
                            _context.SaveChangesAsync();
                        }
                        else
                        {
                            return Json(new { success = false, message = "هذه البطاقة مضافة مسبقاً" });


                        }
                    }
                }

                return RedirectToAction("GetLessons", "Lessons");
            }
            GetUserRole(); 
            return View();

        }

    }
}
