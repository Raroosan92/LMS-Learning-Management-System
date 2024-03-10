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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Protocols;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize]


    public class LessonsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly LMSContext _context;
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();

        string time;
        string Year;
        string Month;
        DateTime Jor;
        public LessonsController(LMSContext context, Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _hostingEnvironment = hostingEnvironment;

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

        [Authorize(Roles = "teacher,admin")]
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
            GetUserRole();

            //var dd = new SelectList(_context.Classes, "Id", "Descriptions");
            List<Subject> Subject_List = new List<Subject>();
            List<Class> Class_List = new List<Class>();
            var dd = _context.VTechersInfos.ToList().ToList();
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
            return View();
        }

        [Authorize(Roles = "admin")]
        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UrlVideo,ClassId,SubjectId,Status,CreatedUser,CreatedDate,TeacherID,Semester")] Lesson lesson, List<IFormFile> multiplefileupload, List<IFormFile> multiplefileupload2)
        {
            if (ModelState.IsValid)
            {


                GetTime();
                lesson.CreatedDate = DateTime.Parse(time);
                lesson.CreatedUser = HttpContext.User.Identity.Name;
                _context.Add(lesson);
                await _context.SaveChangesAsync();


                string lesson_name = lesson.Name;
                string lesson_subject = lesson.Subjectdesc;

                //***********************MULTI DOCS***********************
                // Upload multiple
                int w = 0;
                foreach (var file in multiplefileupload)
                {
                    w = w + 1;
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var fileName = Path.GetFileName(file.FileName);

                        // Generate a unique identifier for the new file name
                        var uniqueFileName = $"{lesson_name}{lesson_subject}_WorkSheet_{w}{fileExtension}";

                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", uniqueFileName);

                        // Check if the "ProductsImageuploads" folder exists, and create it if not
                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var LessonDoc1 = new Document
                        {
                            WorkingSheets = "/Documents/" + uniqueFileName,
                            LessonId = GetMaxID(),

                        };

                        _context.Documents.Add(LessonDoc1);
                        await _context.SaveChangesAsync();
                    }
                }

                //***********************MULTI DOCS***********************


                //***********************MULTI DOCS***********************
                // Upload multiple
                int b = 0;
                foreach (var file in multiplefileupload2)
                {
                    b = b + 1;
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var fileName = Path.GetFileName(file.FileName);

                        // Generate a unique identifier for the new file name
                        var uniqueFileName = $"{lesson_name}{lesson_subject}_Booklet_{b}{fileExtension}";

                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", uniqueFileName);

                        // Check if the "ProductsImageuploads" folder exists, and create it if not
                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var LessonDoc = new Document
                        {
                            Booklets = "/Documents/" + uniqueFileName,
                            LessonId = GetMaxID(),

                        };

                        _context.Documents.Add(LessonDoc);
                        await _context.SaveChangesAsync();
                    }
                }

                //***********************MULTI DOCS***********************

                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Lessons.Where(r => r.Status == true), "Id", "Descriptions", lesson.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation", lesson.SubjectId);


            GetUserRole();
            return View(lesson);
        }
        public int GetMaxID()
        {
            SqlConnection constr = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using (constr)
            {
                SqlCommand cmd = new SqlCommand();
                constr.Open();
                cmd.Connection = constr;
                cmd.CommandText = "select IDENT_CURRENT('Lessons');";
                cmd.CommandType = CommandType.Text;
                var ID = cmd.ExecuteScalar();
                constr.Close();
                return int.Parse(ID.ToString());
            }
            //_context.Database.ExecuteSqlInterpolated($"EXEC GetSaleQty {mrkno}, {itmno}");
        }
        [Authorize(Roles = "admin")]
        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lesson = await _context.Lessons.Include("Documents").FirstOrDefaultAsync(x => x.Id == id.Value);

            //var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            List<Subject> Subject_List = new List<Subject>();
            List<Class> Class_List = new List<Class>();

            if (User.IsInRole("admin"))
            {
                ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions");
                ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");
                ViewData["TeacherID"] = new SelectList(_context.VTechersInfos.Where(r => r.ClassId == lesson.ClassId && r.SubjectId == lesson.SubjectId), "Id", "FullName");



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

            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [Authorize(Roles = "admin")]
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UrlVideo,ClassId,SubjectId,Status,CreatedUser,CreatedDate,TeacherID,Semester")] Lesson lesson, List<IFormFile> multiplefileupload, List<IFormFile> multiplefileupload2)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }
            string lesson_name = lesson.Name;
            string lesson_subject = lesson.Subjectdesc;

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(lesson);
                    _context.Entry(lesson).State = EntityState.Modified;
                    _context.Entry(lesson).Property(x => x.CreatedDate).IsModified = false;
                    _context.Entry(lesson).Property(x => x.CreatedUser).IsModified = false;
                    await _context.SaveChangesAsync();

                    //***********************MULTI DOCS***********************
                    // Upload multiple
                    var directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents");

                    string searchTerm = "WorkSheet";
                    var matchingFiles = Directory.GetFiles(directoryPath)
                                                             .Where(file => Path.GetFileName(file).Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    // Extract and find the maximum number from the file names
                    int? maxNumber = matchingFiles.Select(file =>
                    {
                        Match match = Regex.Match(Path.GetFileName(file), searchTerm + @"_(\d+)", RegexOptions.IgnoreCase);
                        return match.Success ? (int?)int.Parse(match.Groups[1].Value) : null;
                    })
                    .Max();
                    int fileCount = matchingFiles.Count();

                    foreach (var file in multiplefileupload)
                    {
                        if (file.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(file.FileName);
                            var fileName = Path.GetFileName(file.FileName);

                            // Generate a unique identifier for the new file name
                            var uniqueFileName = $"{lesson_name}{lesson_subject}_WorkSheet_{maxNumber + 1}{fileExtension}";

                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", uniqueFileName);

                            // Check if the "ProductsImageuploads" folder exists, and create it if not
                            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                            }

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            var LessonDoc1 = new Document
                            {
                                WorkingSheets = "/Documents/" + uniqueFileName,
                                LessonId = id,

                            };

                            _context.Documents.Add(LessonDoc1);
                            await _context.SaveChangesAsync();
                        }
                    }

                    //***********************MULTI DOCS***********************


                    //***********************MULTI DOCS***********************
                    // Upload multiple
                    var directoryPath2 = Path.Combine(_hostingEnvironment.WebRootPath, "Documents");

                    string searchTerm2 = "Booklet";
                    var matchingFiles2 = Directory.GetFiles(directoryPath2)
                                                             .Where(file => Path.GetFileName(file).Contains(searchTerm2, StringComparison.OrdinalIgnoreCase));
                    // Extract and find the maximum number from the file names
                    int? maxNumber2 = matchingFiles2.Select(file =>
                    {
                        Match match2 = Regex.Match(Path.GetFileName(file), searchTerm2 + @"_(\d+)", RegexOptions.IgnoreCase);
                        return match2.Success ? (int?)int.Parse(match2.Groups[1].Value) : null;
                    })
                    .Max();
                    int fileCount2 = matchingFiles2.Count();

                    foreach (var file in multiplefileupload2)
                    {

                        if (file.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(file.FileName);
                            var fileName = Path.GetFileName(file.FileName);

                            // Generate a unique identifier for the new file name
                            var uniqueFileName = $"{lesson_name}{lesson_subject}_Booklet_{maxNumber2 + 1}{fileExtension}";

                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", uniqueFileName);

                            // Check if the "ProductsImageuploads" folder exists, and create it if not
                            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                            }

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            var LessonDoc = new Document
                            {
                                Booklets = "/Documents/" + uniqueFileName,
                                LessonId = id,

                            };

                            _context.Documents.Add(LessonDoc);
                            await _context.SaveChangesAsync();
                        }
                    }

                    //***********************MULTI DOCS***********************

                }
                catch (DbUpdateConcurrencyException ex)
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
            ViewData["ClassId"] = new SelectList(_context.Lessons.Where(r => r.Status == true), "Id", "Descriptions", lesson.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation", lesson.SubjectId);
            ViewData["TeacherID"] = new SelectList(_context.VTechersInfos.Where(r => r.ClassId == lesson.ClassId && r.SubjectId == lesson.SubjectId), "Id", "FullName");


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
        [Authorize(Roles = "teacher,admin")]

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
                    var x = _context.Lessons.Where(r => r.SubjectId == teacher_Lessons[i].SubjectId && r.ClassId == teacher_Lessons[i].ClassId).OrderByDescending(r => r.Id).FirstOrDefault();
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
                if (stList[i].Semester == 1)
                {
                    stList[i].SemesterDesc = "الفصل الأول";

                }
                else
                {
                    stList[i].SemesterDesc = "الفصل الثاني";

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




        [HttpPost]
        public JsonResult GetTeachersBySubject(string subjectId, string classId, string Semester)
        {
            if (ModelState.IsValid)
            {
                var cc = _context.VTechersInfos.Where(r => r.SubjectId == int.Parse(subjectId) && r.ClassId == int.Parse(classId)).ToList();

                return new JsonResult(cc.ToList());
            }
            else
            {
                return Json(null, System.Web.Mvc.JsonRequestBehavior.AllowGet);

            }

        }


        [HttpPost]
        public async Task<JsonResult> AssignTeacher(string teacherId, int Card_No, int Subject_id, int Class_id, int Semester)
        {

            if (ModelState.IsValid)
            {
                GetTime();

                var Card_Details = _context.CardSubjects.Where(r => r.CardNo == Card_No && r.SubjectId == Subject_id && r.ClassId == Class_id && r.Semester == Semester).FirstOrDefault();

                if (Card_Details != null && teacherId != null)
                {
                    // Update the properties directly on the tracked entity
                    Card_Details.TeacherId = teacherId;

                    // Exclude specific properties from modification
                    _context.Entry(Card_Details).Property(x => x.CardNo).IsModified = false;
                    _context.Entry(Card_Details).Property(x => x.SubjectId).IsModified = false;
                    _context.Entry(Card_Details).Property(x => x.ClassId).IsModified = false;
                    _context.Entry(Card_Details).Property(x => x.Semester).IsModified = false;
                    _context.Entry(Card_Details).Property(x => x.PaymentAmount).IsModified = false;
                    _context.Entry(Card_Details).Property(x => x.PaymentDate).IsModified = false;
                    _context.Entry(Card_Details).Property(x => x.IsPayment).IsModified = false;
                    await _context.SaveChangesAsync();
                }


                //***********************edit enrollments*************************
                var enrollmentSTD_Details = _context.Enrollments.Where(r => r.SubjectId == Subject_id && r.ClassId == Class_id && r.UserId == User.Identity.GetUserId() && r.Semester == Semester).FirstOrDefault();

                if (enrollmentSTD_Details != null && teacherId != null)
                {
                    // Update the properties directly on the tracked entity
                    enrollmentSTD_Details.TeacherId = teacherId;

                    // Exclude specific properties from modification
                    _context.Entry(enrollmentSTD_Details).Property(x => x.SubjectId).IsModified = false;
                    _context.Entry(enrollmentSTD_Details).Property(x => x.ClassId).IsModified = false;
                    _context.Entry(enrollmentSTD_Details).Property(x => x.CreatedDate).IsModified = false;
                    _context.Entry(enrollmentSTD_Details).Property(x => x.Semester).IsModified = false;
                    _context.Entry(enrollmentSTD_Details).Property(x => x.UserId).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                //***********************edit enrollments*************************

                GetUserRole();
                var successMessage = "تمت العملية بنجاح";
                return new JsonResult(new { Success = true, Message = successMessage });
            }

            var model2 = _context.VTeacherSalesCards.OrderByDescending(r => r.UserName);
            GetUserRole();
            // في حالة الفشل، قم بالتعامل مع الاستثناءات أو الأخطاء هنا
            var errorMessage = "حدث خطأ أثناء تنفيذ العملية";
            return new JsonResult(new { Success = false, Message = errorMessage });
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


        [HttpPost]
        public async Task<IActionResult> Removefile(int Id)
        {
            try
            {
                // Assuming _context is your database context
                var fileToRemove = await _context.Documents.FindAsync(long.Parse(Id.ToString()));

                if (fileToRemove == null)
                {
                    return Json(new { success = false, message = "file not found" });
                }

                // Construct the file path
                string fileName = Path.GetFileName(fileToRemove.WorkingSheets);
                string fileName2 = Path.GetFileName(fileToRemove.Booklets);
                if (fileName != null)
                {
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", fileName);
                    // Check if the file exists before attempting to delete
                    if (System.IO.File.Exists(filePath))
                    {
                        // Delete the file
                        System.IO.File.Delete(filePath);
                    }
                }
                if (fileName2 != null)
                {
                    var filePath2 = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", fileName2);
                    // Check if the file exists before attempting to delete
                    if (System.IO.File.Exists(filePath2))
                    {
                        // Delete the file
                        System.IO.File.Delete(filePath2);
                    }
                }
                //var filePath = imageToRemove.ProductImage1;



                // Logic to remove the image from the database
                _context.Documents.Remove(fileToRemove);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "file removed successfully",
                    removedImageId = fileToRemove.Id,
                    removedImagePath = fileToRemove.Booklets,
                    removedImagePath2 = fileToRemove.WorkingSheets
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { success = false, message = "Error removing file" });
            }
        }
        [Authorize(Roles = "student,admin")]

        ////[HttpPost]
        //public IActionResult ShowLessons(int? ClassId, int? SubjectId, string TeacherId)
        //{
        //    GetUserRole();

        //    if (ClassId == null || SubjectId == null || TeacherId == null)
        //    {
        //        return NotFound();
        //    }

        //    var lessons = _context.VLessonCardsSubjects
        //        .Where(r => r.ClassId == ClassId && r.SubjectId == SubjectId && r.TeacherId == TeacherId)
        //        .OrderBy(r => r.Id)
        //        .ToList();

        //    var lessonViewModel = new VLessonCardsSubject()
        //    {
        //        VLessonCardsSubject_Collection = lessons,
        //        TeacherInfo_Collection = _context.VTechersInfos.ToList()
        //    };
        //    //ViewBag.LessonViewModel = lessonViewModel;
        //    TempData["LessonViewModel"] = lessonViewModel;

        //    if (lessonViewModel == null)
        //    {
        //        return NotFound();
        //    }
        //    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    {
        //        return View("_PartialShowLessons", lessonViewModel);
        //        //return Json(new { success = true, message = "جاري التحويل", data = lessonViewModel });
        //    }
        //    else
        //    {
        //        return View("_PartialShowLessons", lessonViewModel);


        //    }

        //    //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    //{
        //    //    // This is an AJAX request, return a partial view
        //    //    return Json(new { lessonViewModel = lessonViewModel, redirectUrl = "/Lessons/_PartialShowLessons/" });
        //    //    //return Json(new { lessonViewModel = lessonViewModel, redirectUrl = "/Lessons/ShowLessons/"+ClassId+"/"+SubjectId+"/"+TeacherId+"" });

        //    //}
        //    // This is a regular request, return the full view
        //    //return View(lessonViewModel);
        //}

        [HttpPost]
        public IActionResult _PartialShowLessons(int? ClassId, int? SubjectId, string TeacherId, int semester)
        {
            GetUserRole();

            if (ClassId == null || SubjectId == null || TeacherId == null)
            {
                return NotFound();
            }

            var lessons = _context.VLessonCardsSubjects
                .Where(r => r.ClassId == ClassId && r.SubjectId == SubjectId && r.TeacherId == TeacherId && r.Semester == semester)
                .OrderBy(r => r.Id)
                .ToList();

            var lessonViewModel = new VLessonCardsSubject()
            {
                VLessonCardsSubject_Collection = lessons,
                TeacherInfo_Collection = _context.VTechersInfos.ToList()
            };

            if (lessonViewModel == null)
            {
                return NotFound();
            }

            return View(lessonViewModel);
        }

        [HttpPost, ActionName("k8vdUMEW0uLWyai31xmmWhtv")]
        public IActionResult _PartialShowLessons_D(int? ClassId, int? SubjectId, string TeacherId, int semester)
        {
            GetUserRole();

            if (ClassId == null || SubjectId == null || TeacherId == null)
            {
                return NotFound();
            }

            var lessons = _context.VLessonCardsSubjects
                .Where(r => r.ClassId == ClassId && r.SubjectId == SubjectId && r.TeacherId == TeacherId && r.Semester == semester)
                .OrderBy(r => r.Id)
                .ToList();

            var lessonViewModel = new VLessonCardsSubject()
            {
                VLessonCardsSubject_Collection = lessons,
                TeacherInfo_Collection = _context.VTechersInfos.ToList()
            };

            if (lessonViewModel == null)
            {
                return NotFound();
            }

            return View("_PartialShowLessons_D", lessonViewModel);
        }



        //public async Task<IActionResult> ShowLessons(int? ClassId, int? SubjectId, string TeacherId)
        //{
        //    if (ClassId == null || SubjectId == null || TeacherId == null )
        //    {
        //        return NotFound();
        //    }
        //    var lessons =  _context.VLessonCardsSubjects.Where(r => r.ClassId == ClassId && r.SubjectId == SubjectId && r.TeacherId == TeacherId).ToList();

        //    var lesson = new VLessonCardsSubject()
        //    {
        //        VLessonCardsSubject_Collection = lessons.OrderBy(r => r.Id).ToList(),
        //        TeacherInfo_Collection = _context.VTechersInfos.ToList().ToList(),
        //    };
        //    if (lesson == null)
        //    {
        //        return NotFound();
        //    }

        //    GetUserRole();
        //    return View(lesson);
        //}



        [Authorize(Roles = "student,admin")]

        public IActionResult GetSubjects()
        {
            List<VEnrollmentStdDetail> _SubjectsSelectedTeacher = new List<VEnrollmentStdDetail>();
            List<VEnrollmentStdDetail> _SubjectsNotSelectedTeacher = new List<VEnrollmentStdDetail>();

            var Enrollments_Std = _context.VEnrollmentStdDetails.Where(r => r.UserId == User.Identity.GetUserId()).ToList();

            foreach (var item in Enrollments_Std)
            {
                if (item.TeacherId != null)
                {
                    var lesson = _context.VEnrollmentStdDetails.Where(r => r.ClassId == item.ClassId && r.SubjectId == item.SubjectId && r.Subjects_Status == true && r.Classes_Status == true && r.TeacherId == item.TeacherId && r.Semester == item.Semester).SingleOrDefault();
                    if (lesson != null)
                    {
                        //lesson.Classdesc = _context.Classes.Select(r => new { r.Descriptions, r.Id, r.Status }).Where(r => r.Id == lesson.ClassId).FirstOrDefault().Descriptions;
                        //lesson.Subjectdesc = _context.Subjects.Select(r => new { r.Abbreviation, r.Id, r.Status }).Where(r => r.Id == lesson.SubjectId).FirstOrDefault().Abbreviation;
                        _SubjectsSelectedTeacher.Add(lesson);
                    }
                }
                if (item.TeacherId == null)
                {
                    var lesson2 = _context.VEnrollmentStdDetails.Where(r => r.ClassId == item.ClassId && r.SubjectId == item.SubjectId && item.TeacherId == null && r.Semester == item.Semester).FirstOrDefault();
                    if (lesson2 != null)
                    {
                        _SubjectsNotSelectedTeacher.Add(lesson2);
                    }
                }
            }
            List<VEnrollmentStdDetail> mergedList = new List<VEnrollmentStdDetail>();

            mergedList = _SubjectsSelectedTeacher.Concat(_SubjectsNotSelectedTeacher).ToList();

            var model = new VEnrollmentStdDetail()
            {
                VEnrollmentStdDetailt_Collection = mergedList.OrderByDescending(r => r.Name),
                TeacherInfo_Collection = _context.VTechersInfos.ToList().ToList(),
            };


            GetUserRole();
            return View(model);


        }


        //public string GetWPF()
        //{
        //    var result = _context.Codes
        //        .Where(c => c.Id == 1)
        //        .Select(c => c.Discriptions)
        //        .FirstOrDefault();

        //    return result;
        //}
        [Authorize(Roles = "student,admin")]
        [ActionName("9AOs8LhLna9PsTTurRH3a9OK61fHRf")]

        public IActionResult GetSubjects_D()
        {

            string wpfAppIdentifier = HttpContext.Request.Query["X-WPF-App-Identifier"];
            //string dbwpf = GetWPF();
            string dbwpf = "dd";

            //var wpfAppIdenctifier = Request.Cookies["X-WPF-App-Identifier"];
            //var wpfAppIdentifier2 = HttpContext.Session.GetString("X-WPF-App-Identifier");

            if (!string.IsNullOrEmpty(wpfAppIdentifier) && string.Equals(wpfAppIdentifier, dbwpf, StringComparison.OrdinalIgnoreCase))
            {
                
                List<VEnrollmentStdDetail> _SubjectsSelectedTeacher = new List<VEnrollmentStdDetail>();
                List<VEnrollmentStdDetail> _SubjectsNotSelectedTeacher = new List<VEnrollmentStdDetail>();

                var Enrollments_Std = _context.VEnrollmentStdDetails.Where(r => r.UserId == User.Identity.GetUserId()).ToList();

                foreach (var item in Enrollments_Std)
                {
                    if (item.TeacherId != null)
                    {
                        var lesson = _context.VEnrollmentStdDetails.Where(r => r.ClassId == item.ClassId && r.SubjectId == item.SubjectId && r.Subjects_Status == true && r.Classes_Status == true && r.TeacherId == item.TeacherId && r.Semester == item.Semester).SingleOrDefault();
                        if (lesson != null)
                        {
                            //lesson.Classdesc = _context.Classes.Select(r => new { r.Descriptions, r.Id, r.Status }).Where(r => r.Id == lesson.ClassId).FirstOrDefault().Descriptions;
                            //lesson.Subjectdesc = _context.Subjects.Select(r => new { r.Abbreviation, r.Id, r.Status }).Where(r => r.Id == lesson.SubjectId).FirstOrDefault().Abbreviation;
                            _SubjectsSelectedTeacher.Add(lesson);
                        }
                    }
                    if (item.TeacherId == null)
                    {
                        var lesson2 = _context.VEnrollmentStdDetails.Where(r => r.ClassId == item.ClassId && r.SubjectId == item.SubjectId && item.TeacherId == null && r.Semester == item.Semester).FirstOrDefault();
                        if (lesson2 != null)
                        {
                            _SubjectsNotSelectedTeacher.Add(lesson2);
                        }
                    }
                }
                List<VEnrollmentStdDetail> mergedList = new List<VEnrollmentStdDetail>();

                mergedList = _SubjectsSelectedTeacher.Concat(_SubjectsNotSelectedTeacher).ToList();

                var model = new VEnrollmentStdDetail()
                {
                    VEnrollmentStdDetailt_Collection = mergedList.OrderByDescending(r => r.Name),
                    TeacherInfo_Collection = _context.VTechersInfos.ToList().ToList(),
                };


                GetUserRole();
                return View("GetSubjects_D", model);
            }
            else
            {
                // Deny access, the request is from a browser
                return Unauthorized();
            }




        }
    }
}




