using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Controllers
{
    public class TeacherCardsSalesController : Controller
    {

        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();
        private readonly LMSContext _context;
        string time;
        string Year;
        string Month;
        DateTime Jor;

        public TeacherCardsSalesController(LMSContext context, Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden)
        {
            _context = context;
            userManager = userMgr;
            _contextUsers = iden;
        }

        // GET: TeacherCardsSalesController
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
        // GET: Classes
        public async Task<IActionResult> Index()
        {
            GetUserRole();
            return View();
            //return View(await _context.Classes.OrderByDescending(r=>r.Id).ToListAsync());
        }

        [HttpPost]
        public IActionResult GetData()
        {
            List<TeacherSalesCard> stList = new List<TeacherSalesCard>();

            if (User.IsInRole("admin"))
            {
                stList = _context.TeacherSalesCards.OrderByDescending(r => r.UserName).ToList();

            
            }
            return new JsonResult(new { data = stList });

        }
        // GET: TeacherCardsSalesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TeacherCardsSalesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TeacherCardsSalesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TeacherCardsSalesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TeacherCardsSalesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TeacherCardsSalesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TeacherCardsSalesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
