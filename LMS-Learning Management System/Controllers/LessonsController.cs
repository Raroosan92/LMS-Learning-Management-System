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
    public class LessonsController : Controller
    {
        private readonly LMSContext _context;
        string time;
        string Year;
        string Month;
        DateTime Jor;
        public LessonsController(LMSContext context)
        {
            _context = context;
        }

        // GET: Lessons
        public async Task<IActionResult> Index()
        {
            return View();
            //var lMSContext = _context.Lessons.Include(l => l.Class).Include(l => l.Subject);
            //return View(await lMSContext.ToListAsync());
        }

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

        // GET: Lessons/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation");
            return View();
        }

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
            return View(lesson);
        }

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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", lesson.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", lesson.SubjectId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
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

        [HttpPost]
        public IActionResult GetData()
        {

            var stList = _context.Lessons.Include(l => l.Class).Include(l => l.Subject).OrderByDescending(r => r.Id).ToList();
           
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
    }
}
