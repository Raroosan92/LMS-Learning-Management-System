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
                ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions");
                ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");

            }
            else
            {
                var teacher_subjects = _context.TeacherEnrollments.Select(r => new { r.SubjectId, r.UserId }).Where(r => r.UserId == User.Identity.GetUserId()).Distinct().ToList();
                for (int i = 0; i < teacher_subjects.Count; i++)
                {
                    var x = _context.Subjects.Where(r => r.Id == teacher_subjects[i].SubjectId && r.Status == true).OrderByDescending(r => r.Id).FirstOrDefault();
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
                ViewData["SubjectId"] = new SelectList(Subject_List.Where(r => r.Status == true), "Id", "Abbreviation");
                ViewData["ClassId"] = new SelectList(Class_List.Where(r => r.Status == true), "Id", "Descriptions");
            }

            ViewData["UserId"] = new SelectList(_context.AspNetUsers.Where(r => r.UserType == "7c72ca3d-4714-4340-b0d0-99cc56ef6623"), "Id", "UserName");
            GetUserRole();
            return View();
        }
        [Authorize(Roles = "admin")]

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassId,SubjectId,UserId,CreatedDate,Semester,CardNo")] Enrollment enrollment)
        {
            //if (ModelState.IsValid)
            //{
            //    GetTime();
            //    enrollment.CreatedDate = DateTime.Parse(time);
            //    _context.Add(enrollment);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions", enrollment.ClassId);
            //ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation", enrollment.SubjectId);
            //ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", enrollment.UserId);
            //GetUserRole();
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
                ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions");
                ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");

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
                ViewData["SubjectId"] = new SelectList(Subject_List.Where(r => r.Status == true), "Id", "Abbreviation");
                ViewData["ClassId"] = new SelectList(Class_List.Where(r => r.Status == true), "Id", "Descriptions");
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassId,SubjectId,UserId,CardNo")] Enrollment enrollment)
        {
            //if (id != enrollment.Id)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(enrollment);
            //        _context.Entry(enrollment).State = EntityState.Modified;
            //        _context.Entry(enrollment).Property(x => x.CreatedDate).IsModified = false;
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!EnrollmentExists(enrollment.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
                return RedirectToAction(nameof(Index));
            //}
            ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions", enrollment.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation", enrollment.SubjectId);
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
            ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers.Where(r => r.UserType == "7c72ca3d-4714-4340-b0d0-99cc56ef6623"), "Id", "UserName");
            TempData.Remove("AlertMessage1");
            GetUserRole();
            return View();
        }

        public List<SelectListItem> _Classes { get; set; }
        public List<SelectListItem> _Subjecs { get; set; }

        List<string> _ClassesLst = new List<string>();
        List<string> _SubjecsLst = new List<string>();


        [Authorize(Roles = "student,admin")]

        public async Task<IActionResult> EnrollSTD1(Mixed_Enrollments_CardDetails mixed_Enrollments_CardDetails)
        {
            var Check_EnrollmentExists = true;
            var semester = mixed_Enrollments_CardDetails.Semester;
            Enrollment enrollment2 = new Enrollment();

            if (ModelState.IsValid)
            {
                GetTime();
                var Card_Details = _context.Cards.Where(r => r.CardNo == mixed_Enrollments_CardDetails.Card_No && r.CardPassword == mixed_Enrollments_CardDetails.Password_Card).FirstOrDefault();
                if (Card_Details.CardNo > 0)
                {


                    //*************To assign subjects to card***************************

                    var CardSubjects = new CardSubject();

                    _Classes = (from Classes1 in _context.Classes
                                select new SelectListItem
                                {
                                    Text = Classes1.Descriptions,
                                    Value = Classes1.Id.ToString()
                                }).ToList();



                    _Subjecs = (from Subjects1 in _context.Subjects
                                select new SelectListItem
                                {
                                    Text = Subjects1.Name,
                                    Value = Subjects1.Id.ToString()
                                }).ToList();


                    _Classes = _Classes.OrderBy(v => v.Value).Distinct().ToList();
                    _Subjecs = _Subjecs.OrderBy(v => v.Value).Distinct().ToList();

                    string[] ClassId = Request.Form["lstClasses"].ToString().Split(",");
                    string[] SubjecsId = Request.Form["lstSubjecs"].ToString().Split(",");



                    foreach (string id in ClassId)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            string name = _Classes.Where(x => x.Value == id).FirstOrDefault().Text;
                            _ClassesLst.Add(id);
                        }
                    }

                    foreach (string id2 in SubjecsId)
                    {
                        if (!string.IsNullOrEmpty(id2))
                        {
                            string name = _Classes.Where(x => x.Value == id2).FirstOrDefault().Text;
                            _SubjecsLst.Add(id2);
                        }
                    }


                    for (int a = 0; a < _ClassesLst.Count; a++)
                    {
                        for (int b = 0; b < _SubjecsLst.Count; b++)
                        {
                            Check_EnrollmentExists = _context.Enrollments.Any(r => r.Semester == semester && r.UserId == User.Identity.GetUserId() && r.SubjectId == int.Parse(_SubjecsLst[b]) && r.ClassId == int.Parse(_ClassesLst[a]));

                            if (Check_EnrollmentExists == false)
                            {

                                CardSubjects = new CardSubject
                                {
                                    CardNo = Card_Details.Id,
                                    SubjectId = int.Parse(_SubjecsLst[b]),
                                    ClassId = int.Parse(_ClassesLst[a]),
                                    Semester = semester
                                };
                                _context.CardSubjects.Add(CardSubjects);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                TempData["AlertMessage1"] = "المواد التي تم اختيارها مسجلة مسبقا لنفس الصف";
                                return View("EnrollSTD");
                            }

                        }
                    }

                    //*************To assign subjects to card***************************
                    int cardid = 0;

                    var card_Enrollments = _context.CardSubjects.Where(r => r.CardNo == Card_Details.Id).ToList();
                    foreach (var item in card_Enrollments)
                    {
                        cardid = item.CardNo;
                        if (Check_EnrollmentExists == false)
                        {

                            enrollment2 = new Enrollment
                            {
                                ClassId = item.ClassId,
                                SubjectId = item.SubjectId,
                                UserId = User.Identity.GetUserId(),
                                CreatedDate = DateTime.Parse(time),
                                Semester = semester,
                                CardNo = Card_Details.Id
                            };
                            _context.Add(enrollment2);
                            await _context.SaveChangesAsync();



                        }
                        else
                        {
                            Check_EnrollmentExists = true;
                            return Json(new { success = false, message = "هذه البطاقة مضافة مسبقاً" });


                        }
                    }
                    if (Check_EnrollmentExists == false)
                    {

                        var Card_Details2 = _context.Cards
    .Where(r => r.CardNo == mixed_Enrollments_CardDetails.Card_No && r.CardPassword == mixed_Enrollments_CardDetails.Password_Card)
    .FirstOrDefault();

                        if (Card_Details2 != null)
                        {
                            // Update the properties directly on the tracked entity
                            Card_Details2.UserName = User.Identity.GetUserName();
                            Card_Details2.UserId = User.Identity.GetUserId();

                            // Exclude specific properties from modification
                            _context.Entry(Card_Details2).Property(x => x.CardPassword).IsModified = false;
                            _context.Entry(Card_Details2).Property(x => x.CardNo).IsModified = false;
                            _context.Entry(Card_Details2).Property(x => x.CardPrice).IsModified = false;
                            _context.Entry(Card_Details2).Property(x => x.CardStatus).IsModified = false;
                            _context.Entry(Card_Details2).Property(x => x.NumberOfSubjects).IsModified = false;

                            await _context.SaveChangesAsync();
                        }


                        //var Card_Details2 = _context.Cards.Where(r => r.CardNo == mixed_Enrollments_CardDetails.Card_No && r.CardPassword == mixed_Enrollments_CardDetails.Password_Card).FirstOrDefault();

                        //Card cards = new Card

                        //{
                        //    Id = int.Parse(Card_Details2.Id.ToString()),
                        //    UserName = User.Identity.GetUserName(),
                        //    UserId = User.Identity.GetUserId(),
                        //};
                        //_context.Attach(Card_Details2);


                        //_context.Entry(cards).State = EntityState.Modified;
                        //_context.Entry(cards).Property(x => x.CardPassword).IsModified = false;
                        //_context.Entry(cards).Property(x => x.CardNo).IsModified = false;
                        //_context.Entry(cards).Property(x => x.CardPrice).IsModified = false;
                        //_context.Entry(cards).Property(x => x.CardStatus).IsModified = false;
                        //_context.Entry(cards).Property(x => x.NumberOfSubjects).IsModified = false;

                        //await _context.SaveChangesAsync();
                    }

                }

                return RedirectToAction("GetSubjects", "Lessons");
            }
            GetUserRole();
            return View();

        }
        [HttpPost]
        public JsonResult Check_Crad(string Card_No, string Password_Card)
        {
            if (ModelState.IsValid)
            {
                var Card_Details = _context.Cards.Where(r => r.CardNo == int.Parse(Card_No) && r.CardPassword == Password_Card);

                return new JsonResult(Card_Details.ToList());
            }
            else
            {
                return Json(null, System.Web.Mvc.JsonRequestBehavior.AllowGet);

            }

        }


    }
}
