using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS_Learning_Management_System.Models;
using Microsoft.AspNetCore.Authorization;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize(Roles = "admin")]

    public class ActiveSessionsController : Controller
    {
        private readonly LMSContext _context;

        public ActiveSessionsController(LMSContext context)
        {
            _context = context;
        }

        // GET: ActiveSessions
        public async Task<IActionResult> Index()
        {
            return View(await _context.ActiveSessions.ToListAsync());
        }

        // GET: ActiveSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activeSession = await _context.ActiveSessions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activeSession == null)
            {
                return NotFound();
            }

            return View(activeSession);
        }

        //// GET: ActiveSessions/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: ActiveSessions/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,UserId,UserName,PhoneNumber,LoginDate,DeviceType,MacAddress,ComputerName")] ActiveSession activeSession)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(activeSession);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(activeSession);
        //}

        //// GET: ActiveSessions/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var activeSession = await _context.ActiveSessions.FindAsync(id);
        //    if (activeSession == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(activeSession);
        //}

        //// POST: ActiveSessions/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,UserName,PhoneNumber,LoginDate,DeviceType,MacAddress,ComputerName")] ActiveSession activeSession)
        //{
        //    if (id != activeSession.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(activeSession);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ActiveSessionExists(activeSession.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(activeSession);
        //}

        //// GET: ActiveSessions/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var activeSession = await _context.ActiveSessions
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (activeSession == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(activeSession);
        //}

        //// POST: ActiveSessions/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var activeSession = await _context.ActiveSessions.FindAsync(id);
        //    _context.ActiveSessions.Remove(activeSession);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}


        [HttpPost]
        public IActionResult GetData()
        {
            List<ActiveSession> stList = new List<ActiveSession>();

            stList = _context.ActiveSessions.OrderByDescending(r => r.UserName).ToList();

            return new JsonResult(new { data = stList });

        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (LMSContext db = new LMSContext())
            {
                ActiveSession std = db.ActiveSessions.Where(x => x.Id == id).FirstOrDefault<ActiveSession>();
                db.ActiveSessions.Remove(std);
                db.SaveChanges();
                return Json(new { success = true, message = "تمت عملية الحذف بنجاح" });
            }
        }
        //private bool ActiveSessionExists(int id)
        //{
        //    return _context.ActiveSessions.Any(e => e.Id == id);
        //}
    }
}
