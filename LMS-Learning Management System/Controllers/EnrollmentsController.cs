using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS_Learning_Management_System.Models;

namespace LMS_Learning_Management_System.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly LMSContext _context;
        string time;
        string Year;
        string Month;
        DateTime Jor;
        public EnrollmentsController(LMSContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index()
        {
            return View();
            //var lMSContext = _context.Enrollments.Include(e => e.Class).Include(e => e.Subject).Include(e => e.User);
            //return View(await lMSContext.ToListAsync());
        }

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

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName");
            return View();
        }

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
            return View(enrollment);
        }

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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", enrollment.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", enrollment.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", enrollment.UserId);
            return View(enrollment);
        }

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
        public IActionResult GetData()
        {
            var stList = _context.Enrollments.Include(e => e.Class).Include(e => e.Subject).Include(e => e.User).OrderByDescending(r => r.Id).ToList();
            for (int i = 0; i < stList.Count; i++)
            {
                var subjects = _context.Subjects.Where(r => r.Id == stList[i].SubjectId).SingleOrDefault();
                var classes = _context.Classes.Where(r => r.Id == stList[i].ClassId).SingleOrDefault();
                var users = _context.AspNetUsers.Where(r => r.Id== stList[i].UserId).SingleOrDefault();
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
    }
}
