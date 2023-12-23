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
using Microsoft.AspNetCore.Identity;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize]


    public class LessonsController : Controller
    {
        private readonly LMSContext _context;
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();

        string time;
        string Year;
        string Month;
        DateTime Jor;
        public LessonsController(LMSContext context, Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden)
        {
            _context = context;
            // BaseController aa = new BaseController(userMgr,iden);
            //aa.GetUserRole();

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
        [Authorize(Roles = "admin,teacher")]
        // GET: Lessons
        public async Task<IActionResult> Index()
        {
            GetUserRole();
            return View();
            //var lMSContext = _context.Lessons.Include(l => l.Class).Include(l => l.Subject);
            //return View(await lMSContext.ToListAsync());
        }

    [Authorize(Roles = "admin")]
        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Class)
                .Include(l => l.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

    [Authorize(Roles = "admin")]
        // GET: Lessons/Create
        public IActionResult Create()
        {
            //var dd = new SelectList(_context.Classes, "Id", "Descriptions");
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
            GetUserRole();
            return View();
        }

    [Authorize(Roles = "admin")]
        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UrlVideo,ClassId,SubjectId,Status,CreatedUser,CreatedDate")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                GetTime();
                lesson.CreatedDate = DateTime.Parse(time);
                lesson.CreatedUser = HttpContext.User.Identity.Name;
                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Lessons, "Id", "Descriptions", lesson.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", lesson.SubjectId);
            GetUserRole(); 
            return View(lesson);
        }

    [Authorize(Roles = "admin")]
        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
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

            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
    [Authorize(Roles = "admin")]
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UrlVideo,ClassId,SubjectId,Status")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    _context.Entry(lesson).State = EntityState.Modified;
                    _context.Entry(lesson).Property(x => x.CreatedDate).IsModified = false;
                    _context.Entry(lesson).Property(x => x.CreatedUser).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Lessons, "Id", "Descriptions", lesson.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", lesson.SubjectId);
            return View(lesson);
        }

        //// GET: Lessons/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var lesson = await _context.Lessons
        //        .Include(l => l.Class)
        //        .Include(l => l.Subject)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (lesson == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(lesson);
        //}

        //// POST: Lessons/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var lesson = await _context.Lessons.FindAsync(id);
        //    _context.Lessons.Remove(lesson);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [Authorize(Roles = "admin")]

        [HttpPost]
        public IActionResult GetData()
        {

            List<Lesson> stList = new List<Lesson>();

            if (User.IsInRole("admin"))
            {
                  stList = _context.Lessons.Include(l => l.Class).Include(l => l.Subject).OrderByDescending(r => r.Id).ToList();

            }
            else
            {
                var teacher_Lessons = _context.TeacherEnrollments.Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_Lessons.Count; i++)
                {
                    var x = _context.Lessons.Where(r => r.SubjectId == teacher_Lessons[i].SubjectId && r.ClassId== teacher_Lessons[i].ClassId).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (x != null)
                    {
                    stList.Add(x);
                    }

                }
            }
            for (int i = 0; i < stList.Count; i++)
            {
            var subjects = _context.Subjects.Where(r=>r.Id== stList[i].SubjectId).SingleOrDefault();
            var classes = _context.Classes.Where(r => r.Id == stList[i].ClassId).SingleOrDefault();
                stList[i].Classdesc = classes.Descriptions;
                stList[i].Subjectdesc = subjects.Name;
                if (stList[i].Status == true)
                {
                    stList[i].Status2 = "فعال";

                }
                else
                {
                    stList[i].Status2 = "غير فعال";

                }
            }
            return new JsonResult(new { data = stList });

        }
    [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (LMSContext db = new LMSContext())
            {
                Lesson std = db.Lessons.Where(x => x.Id == id).FirstOrDefault<Lesson>();
                db.Lessons.Remove(std);
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
        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
        [Authorize(Roles = "student,admin")]

        public IActionResult GetLessons()
        {
            //return View();
            //var lMSContext = _context.Lessons.Include(l => l.Class).Include(l => l.Subject);
            //var Lessons = new Lesson();
            List<Lesson> _Lessons = new List<Lesson>();

            var Enrollments_Std = _context.Enrollments.Include(l => l.Class).Include(l => l.Subject).Where(r=>r.UserId== User.Identity.GetUserId());

            foreach (var item in Enrollments_Std)
            {
                var lesson = _context.Lessons.Where(r=>r.ClassId==item.ClassId && r.SubjectId==item.SubjectId && r.Status == true).FirstOrDefault();
                if (lesson != null)
                {
                    lesson.Classdesc = _context.Classes.Select(r=>new { r.Descriptions,r.Id,r.Status }).Where(r => r.Id == lesson.ClassId).FirstOrDefault().Descriptions;
                    lesson.Subjectdesc = _context.Subjects.Select(r=>new { r.Abbreviation,r.Id,r.Status }).Where(r => r.Id == lesson.SubjectId).FirstOrDefault().Abbreviation;
                _Lessons.Add(lesson);
                }
            }
            GetUserRole();
            return View(_Lessons);
        }

        [Authorize(Roles = "student,admin")]

        // GET: Lessons/Details/5
        public async Task<IActionResult> ShowLesson(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Class)
                .Include(l => l.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }
            GetUserRole();
            return View(lesson);
        }
    }
}
