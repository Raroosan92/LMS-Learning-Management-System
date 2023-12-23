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
    [Authorize(Roles = "admin")]
    public class SubjectsController : Controller
    {
        private readonly LMSContext _context; 
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();

        string time;
        string Year;
        string Month;
        DateTime Jor;
        public SubjectsController(LMSContext context, Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden)
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

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            GetUserRole();
            return View();
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject.Status == true)
            {
                subject.Status2 = "فعال";

            }
            else
            {
                subject.Status2 = "غير فعال";

            }
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            GetUserRole();
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Abbreviation,Status,CreatedDate,CreatedUser")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                GetTime();

                subject.CreatedDate = DateTime.Parse(time);
                subject.CreatedUser = HttpContext.User.Identity.Name;
                _context.Add(subject);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            GetUserRole();
            return View(subject);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Abbreviation,Status")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    _context.Entry(subject).State = EntityState.Modified;
                    _context.Entry(subject).Property(x => x.CreatedDate).IsModified = false;
                    _context.Entry(subject).Property(x => x.CreatedUser).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
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
            return View(subject);
        }

        [HttpPost]
        public IActionResult GetData()
        {
            List<Subject> stList = new List<Subject>();

            if (User.IsInRole("admin"))
            {
                stList = _context.Subjects.OrderByDescending(r => r.Id).ToList();

            }
            else
            {
                var teacher_subjects = _context.TeacherEnrollments.Select(r=>new { r.SubjectId,r.UserId}).Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_subjects.Count; i++)
                {
                    var x = _context.Subjects.Where(r => r.Id == teacher_subjects[i].SubjectId).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (x != null)
                    {
                        stList.Add(x);
                    }

                }
            }
            for (int i = 0; i < stList.Count; i++)
            {
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
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (LMSContext db = new LMSContext())
            {
                Subject std = db.Subjects.Where(x => x.Id == id).FirstOrDefault<Subject>();
                db.Subjects.Remove(std);
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

        //// GET: Subjects/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var subject = await _context.Subjects
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (subject == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(subject);
        //}

        //// POST: Subjects/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var subject = await _context.Subjects.FindAsync(id);
        //    _context.Subjects.Remove(subject);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
