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
    public class CardsController : Controller
    {
        private readonly LMSContext _context;
        string time;
        string Year;
        string Month;
        DateTime Jor;
        public CardsController(LMSContext context)
        {
            _context = context;
        }

        // GET: Cards
        public async Task<IActionResult> Index()
        {
            return View();
            //var lMSContext = _context.Cards.Include(c => c.Class).Include(c => c.Subject).Include(c => c.User);
            //return View(await lMSContext.ToListAsync());
        }

        // GET: Cards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .Include(c => c.Class)
                .Include(c => c.Subject)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);


            var subjects = _context.Subjects.Where(r => r.Id == card.SubjectId).SingleOrDefault();
            var classes = _context.Classes.Where(r => r.Id == card.ClassId).SingleOrDefault();
            var users = _context.AspNetUsers.Where(r => r.Id == card.UserId).SingleOrDefault();
            card.Classdesc = classes.Descriptions;
            card.Subjectdesc = subjects.Name;
            card.Userdesc = users.UserName;
            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // GET: Cards/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: Cards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CardNo,CardPassword,CardStatus,UserId,UserName,SubjectId,ClassId")] Card card)
        {
            if (ModelState.IsValid)
            {
                _context.Add(card);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", card.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", card.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", card.UserId);
            return View(card);
        }

        // GET: Cards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", card.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", card.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", card.UserId);
            return View(card);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CardNo,CardPassword,CardStatus,UserId,UserName,SubjectId,ClassId")] Card card)
        {
            if (id != card.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(card);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardExists(card.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", card.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", card.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", card.UserId);
            return View(card);
        }

        //// GET: Cards/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var card = await _context.Cards
        //        .Include(c => c.Class)
        //        .Include(c => c.Subject)
        //        .Include(c => c.User)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (card == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(card);
        //}

        //// POST: Cards/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var card = await _context.Cards.FindAsync(id);
        //    _context.Cards.Remove(card);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        public IActionResult GetData()
        {
            var stList = _context.Cards.Include(c => c.Class).Include(c => c.Subject).Include(c => c.User).ToList();
            for (int i = 0; i < stList.Count; i++)
            {
                var subjects = _context.Subjects.Where(r => r.Id == stList[i].SubjectId).SingleOrDefault();
                var classes = _context.Classes.Where(r => r.Id == stList[i].ClassId).SingleOrDefault();
                var users = _context.AspNetUsers.Where(r => r.Id == stList[i].UserId).SingleOrDefault();
                stList[i].Classdesc = classes.Descriptions;
                stList[i].Subjectdesc = subjects.Name;
                stList[i].Userdesc = users.UserName;

                if (stList[i].CardStatus == true)
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

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.Id == id);
        }
    }
}
