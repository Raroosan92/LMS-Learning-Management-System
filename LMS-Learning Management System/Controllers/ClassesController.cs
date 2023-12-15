using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;

namespace LMS_Learning_Management_System.Controllers
{
    public class ClassesController : Controller
    {
        private readonly LMSContext _context;
        string time;
        string Year;
        string Month;
        DateTime Jor;

        public ClassesController(LMSContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            return View();
            //return View(await _context.Classes.OrderByDescending(r=>r.Id).ToListAsync());
        }
       
        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@class.Status == true)
            {
                @class.Status2 = "فعال";

            }
            else
            {
                @class.Status2 = "غير فعال";

            }
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descriptions,Status,CreatedDate,CreatedUser")] Class @class)
        {
            if (ModelState.IsValid)
            {
                GetTime();
                @class.CreatedDate = DateTime.Parse(time);
                @class.CreatedUser = HttpContext.User.Identity.Name;
                _context.Add(@class);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);


            if (@class == null)
            {
                return NotFound();
            }
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descriptions,Status")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    _context.Entry(@class).State = EntityState.Modified;
                    _context.Entry(@class).Property(x => x.CreatedDate).IsModified = false;
                    _context.Entry(@class).Property(x => x.CreatedUser).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            return View(@class);
        }

        //// GET: Classes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var @class = await _context.Classes
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (@class == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(@class);
        //}

        //// POST: Classes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var @class = await _context.Classes.FindAsync(id);
        //    _context.Classes.Remove(@class);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        public IActionResult GetData()
        {
            List<Class> stList = new List<Class>();

            if (User.IsInRole("admin"))
            {
                stList = _context.Classes.OrderByDescending(r => r.Id).ToList();

            }
            else
            {
                var teacher_Classes = _context.TeacherEnrollments.Select(r => new { r.ClassId, r.UserId }).Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_Classes.Count; i++)
                {
                    var x = _context.Classes.Where(r => r.Id == teacher_Classes[i].ClassId).OrderByDescending(r => r.Id).FirstOrDefault();
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
                Class std = db.Classes.Where(x => x.Id == id).FirstOrDefault<Class>();
                db.Classes.Remove(std);
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
        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }


    }
}
