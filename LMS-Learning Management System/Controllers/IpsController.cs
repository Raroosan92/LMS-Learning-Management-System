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
    public class IpsController : Controller
    {
        private readonly LMSContext _context;

        public IpsController(LMSContext context)
        {
            _context = context;
        }

        // GET: Ips
        public async Task<IActionResult> Index()
        {
            return View(await _context.MachineData.ToListAsync());
        }

        // GET: Ips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineDatum = await _context.MachineData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machineDatum == null)
            {
                return NotFound();
            }

            return View(machineDatum);
        }

        // GET: Ips/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DeviceUserAgent,DevicePlatformName,DevicePlatformProcessor,DeviceEngine,DeviceBrowser,DeviceType,IpAddress,Macaddress,Ip2,Uname,pass")] MachineDatum machineDatum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machineDatum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(machineDatum);
        }

        // GET: Ips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineDatum = await _context.MachineData.FindAsync(id);
            if (machineDatum == null)
            {
                return NotFound();
            }
            return View(machineDatum);
        }

        // POST: Ips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DeviceUserAgent,DevicePlatformName,DevicePlatformProcessor,DeviceEngine,DeviceBrowser,DeviceType,IpAddress,Macaddress,Ip2,Uname,pass")] MachineDatum machineDatum)
        {
            if (id != machineDatum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machineDatum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineDatumExists(machineDatum.Id))
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
            return View(machineDatum);
        }

        // GET: Ips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineDatum = await _context.MachineData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machineDatum == null)
            {
                return NotFound();
            }

            return View(machineDatum);
        }

        // POST: Ips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineDatum = await _context.MachineData.FindAsync(id);
            if (machineDatum != null)
            {
                _context.MachineData.Remove(machineDatum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineDatumExists(int id)
        {
            return _context.MachineData.Any(e => e.Id == id);
        }
    }
}
