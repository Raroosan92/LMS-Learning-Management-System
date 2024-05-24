using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS_Learning_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize(Roles = "admin")]

    public class CardsController : Controller
    {
        private readonly LMSContext _context;
        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        private readonly IConfiguration _configuration;
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();
        string time;
        string Year;
        string Month;
        DateTime Jor;
        public CardsController(LMSContext context, Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden, IConfiguration configuration)
        {
            _configuration = configuration;
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

        // GET: Cards
        public async Task<IActionResult> Index()
        {
            //GenerateAndInsertCards();
            GetUserRole();
            return View();
            // var lMSContext = _context.Cards.Include(c => c.User);
            // return View(await lMSContext.ToListAsync());
        }

        // GET: Cards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var card = GetCards_And_Details(id);
            if (card == null)
            {
                return NotFound();
            }

            return View(card);
            //var card = await _context.Cards.Include(c => c.User)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            ////var subjects = _context.Subjects.Where(r => r.Id == card.SubjectId).SingleOrDefault();
            ////var classes = _context.Classes.Where(r => r.Id == card.ClassId).SingleOrDefault();
            //var users = _context.AspNetUsers.Where(r => r.Id == card.UserId).SingleOrDefault();
            ////card.Classdesc = classes.Descriptions;
            ////card.Subjectdesc = subjects.Name;
            //card.Userdesc = users.UserName;


        }

        // GET: Cards/Create
        public IActionResult Create()
        {
            //ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ClassId"] = new SelectList(_context.Classes.Where(r => r.Status == true), "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Id", "Abbreviation");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers.Where(r => r.UserType == "7c72ca3d-4714-4340-b0d0-99cc56ef6623"), "Id", "UserName");
            GetUserRole();
            return View();
            //return View();
        }
        //public List<SelectListItem> _Classes { get; set; }
        //public List<SelectListItem> _Subjecs { get; set; }

        //List<string> _ClassesLst = new List<string>();
        //List<string> _SubjecsLst = new List<string>();


        // POST: Cards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CardNo,CardPassword,CardPrice,CardStatus,UserId,UserName,NumberOfSubjects")] Card card)
        {
            var cardsExist = _context.Cards.Where(r => r.CardNo == card.CardNo).ToList();
            if (cardsExist.Count == 0)
            {

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Add(card);
                        await _context.SaveChangesAsync();
                        TempData["SweetAlert"] = "success";
                        GetUserRole();
                        return View();

                    }
                    catch (Exception ex)
                    {
                        // في حالة الفشل
                        TempData["SweetAlert"] = "error";
                        GetUserRole();
                        return View();

                    }

                    //var CardSubjects = new CardSubject();

                    //_Classes = (from Classes1 in _context.Classes
                    //            select new SelectListItem
                    //            {
                    //                Text = Classes1.Descriptions,
                    //                Value = Classes1.Id.ToString()
                    //            }).ToList();



                    //_Subjecs = (from Subjects1 in _context.Subjects
                    //            select new SelectListItem
                    //            {
                    //                Text = Subjects1.Name,
                    //                Value = Subjects1.Id.ToString()
                    //            }).ToList();


                    //_Classes = _Classes.OrderBy(v => v.Value).Distinct().ToList();
                    //_Subjecs = _Subjecs.OrderBy(v => v.Value).Distinct().ToList();

                    //string[] ClassId = Request.Form["lstClasses"].ToString().Split(",");
                    //string[] SubjecsId = Request.Form["lstSubjecs"].ToString().Split(",");



                    //foreach (string id in ClassId)
                    //{
                    //    if (!string.IsNullOrEmpty(id))
                    //    {
                    //        string name = _Classes.Where(x => x.Value == id).FirstOrDefault().Text;
                    //        _ClassesLst.Add(id);
                    //    }
                    //}

                    //foreach (string id2 in SubjecsId)
                    //{
                    //    if (!string.IsNullOrEmpty(id2))
                    //    {
                    //        string name = _Classes.Where(x => x.Value == id2).FirstOrDefault().Text;
                    //        _SubjecsLst.Add(id2);
                    //    }
                    //}


                    //for (int a = 0; a < _ClassesLst.Count; a++)
                    //{
                    //    for (int b = 0; b < _SubjecsLst.Count; b++)
                    //    {
                    //        CardSubjects = new CardSubject
                    //        {
                    //            CardNo = card.Id,
                    //            SubjectId = int.Parse(_SubjecsLst[b]),
                    //            ClassId = int.Parse(_ClassesLst[a])
                    //        };
                    //        _context.CardSubjects.Add(CardSubjects);
                    //        await _context.SaveChangesAsync();
                    //    }
                    //}


                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
                //ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", card.ClassId);
                //ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", card.SubjectId);
                //ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", card.UserId);


            }
            else
            {
                GetUserRole();
                TempData["SweetAlert"] = "error";
                return View();

            }
        }

        // GET: Cards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //List<CardSubject> query = (from d in _context.CardSubjects
            //                           join h in _context.Cards on d.CardNo equals h.Id
            //                           select d).ToList();

            //var model = new Mixed_Cards_CardDetails()
            //{
            //    HD_Collection = await _context.Cards.Where(r=>r.Id==id).ToListAsync(),
            //    DTL_Collection = query.AsEnumerable(),
            //    Subject_Collection = _context.Subjects.ToList(),
            //    Class_Collection = _context.Classes.ToList()
            //};

            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }
            //ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", card.ClassId);
            //ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", card.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", card.UserId);
            return View(card);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CardNo,CardPassword,CardPrice,CardStatus,UserId,UserName,NumberOfSubjects")] Card card)
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
                    _context.Entry(card).Property(u => u.UserId).IsModified = false;
                    _context.Entry(card).Property(u => u.UserName).IsModified = false;

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
            //ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", card.ClassId);
            //ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", card.SubjectId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", card.UserId);
            return View(card);
        }

        // GET: Cards/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var card = await _context.Cards
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
            try
            {


                var stList = _context.Cards.ToList();
                for (int i = 0; i < stList.Count; i++)
                {
                    //var subjects = _context.Subjects.Where(r => r.Id == stList[i].SubjectId).SingleOrDefault();
                    //var classes = _context.Classes.Where(r => r.Id == stList[i].ClassId).SingleOrDefault();
                    var users = _context.AspNetUsers.Where(r => r.Id == stList[i].UserId).SingleOrDefault();
                    //stList[i].Classdesc = classes.Descriptions;
                    //stList[i].Subjectdesc = subjects.Name;
                    try
                    {
                        stList[i].Userdesc = users.FullName;

                    }
                    catch (Exception xx)
                    {
                        stList[i].Userdesc = "";

                    }


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
            catch (Exception dd)
            {
                return new JsonResult("Error");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using (LMSContext db = new LMSContext())
            {
                try
                {
                    bool deleted = true;
                    //CardSubject cardsdtl = db.CardSubjects.Where(x => x.CardNo == id).FirstOrDefault<CardSubject>();
                    //db.CardSubjects.Remove(cardsdtl);
                    //db.SaveChanges();
                    Card cards2 = db.Cards.Where(x => x.Id == id).FirstOrDefault<Card>();

                    var cardsSubjects = await db.CardSubjects.Where(x => x.CardNo == cards2.Id).ToListAsync();

                    try
                    {

                        foreach (var item in cardsSubjects)
                        {
                            Enrollment Enrollments = db.Enrollments.Where(x => x.SubjectId == item.SubjectId && x.ClassId == item.ClassId && x.Semester == item.Semester && x.CardNo==item.CardNo).FirstOrDefault<Enrollment>();
                            if (Enrollments != null)
                            {

                                db.Enrollments.Remove(Enrollments);
                                await db.SaveChangesAsync();
                                deleted = true;
                            }
                            else
                            {
                                deleted = true;
                            }

                        }

                    }
                    catch (Exception)
                    {

                        deleted = false;
                    }

                    try
                    {


                        foreach (var item2 in cardsSubjects)
                        {
                            CardSubject cardsSubjects1 = db.CardSubjects.Where(x => x.CardNo == item2.CardNo && x.SubjectId == item2.SubjectId && x.ClassId == item2.ClassId && x.Semester == item2.Semester).FirstOrDefault<CardSubject>();

                            if (cardsSubjects1 != null)
                            {

                                db.CardSubjects.Remove(cardsSubjects1);
                                await db.SaveChangesAsync();
                                deleted = true;
                            }

                        }
                    }
                    catch (Exception)
                    {

                        deleted = false;
                    }
                    if (deleted)
                    {

                        Card cards = db.Cards.Where(x => x.Id == id).FirstOrDefault<Card>();

                        db.Cards.Remove(cards);
                        db.SaveChanges();

                        return Json(new { success = true, message = "تمت عملية الحذف بنجاح" });
                    }
                    else
                    {
                        return Json(new { success = true, message = "تمت عملية الحذف بنجاح" });

                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = true, message = "خطأ في عملية الحذف" });

                }
            }


        }
        
        
        [HttpPost]
        public async Task<IActionResult> MakeNew(int id)
        {
            using (LMSContext db = new LMSContext())
            {
                try
                {
                    bool deleted = true;
                    //CardSubject cardsdtl = db.CardSubjects.Where(x => x.CardNo == id).FirstOrDefault<CardSubject>();
                    //db.CardSubjects.Remove(cardsdtl);
                    //db.SaveChanges();
                    Card cards2 = db.Cards.Where(x => x.Id == id).FirstOrDefault<Card>();

                    var cardsSubjects = await db.CardSubjects.Where(x => x.CardNo == cards2.Id).ToListAsync();

                    try
                    {

                        foreach (var item in cardsSubjects)
                        {
                            Enrollment Enrollments = db.Enrollments.Where(x => x.SubjectId == item.SubjectId && x.ClassId == item.ClassId && x.Semester == item.Semester && x.CardNo==item.CardNo).FirstOrDefault<Enrollment>();
                            if (Enrollments != null)
                            {

                                db.Enrollments.Remove(Enrollments);
                                await db.SaveChangesAsync();
                                deleted = true;
                            }
                            else
                            {
                                deleted = true;
                            }

                        }

                    }
                    catch (Exception)
                    {

                        deleted = false;
                    }

                    try
                    {


                        foreach (var item2 in cardsSubjects)
                        {
                            CardSubject cardsSubjects1 = db.CardSubjects.Where(x => x.CardNo == item2.CardNo && x.SubjectId == item2.SubjectId && x.ClassId == item2.ClassId && x.Semester == item2.Semester).FirstOrDefault<CardSubject>();

                            if (cardsSubjects1 != null)
                            {

                                db.CardSubjects.Remove(cardsSubjects1);
                                await db.SaveChangesAsync();
                                deleted = true;
                            }

                        }
                    }
                    catch (Exception)
                    {

                        deleted = false;
                    }
                    if (deleted)
                    {

                        Card cards = db.Cards.Where(x => x.Id == id).FirstOrDefault<Card>();
                        cards.UserName = null;
                        cards.UserId = null;
                        db.Cards.Update(cards);
                        db.SaveChanges();

                        return Json(new { success = true, message = "تمت عملية التجديد بنجاح" });
                    }
                    else
                    {
                        return Json(new { success = true, message = "تمت عملية التجديد بنجاح" });

                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = true, message = "خطأ في عملية التجديد" });

                }
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



        public Mixed_Cards_CardDetails GetCards_And_Details(int? id)
        {


            List<CardSubject> query = (from d in _context.CardSubjects
                                       join h in _context.Cards on d.CardNo equals h.Id
                                       where h.Id == id
                                       select d).ToList();

            var model = new Mixed_Cards_CardDetails()
            {
                HD_Collection = _context.Cards.ToList(),
                DTL_Collection = query.AsEnumerable(),
                Subject_Collection = _context.Subjects.ToList(),
                Class_Collection = _context.Classes.ToList(),
                Users_Collection= _contextUsers.Users.ToList()
            };
            return (model);

        }




        public void GenerateAndInsertCards()
        {
            // Database connection string
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Generate 12 unique card numbers
            int[] cardNumbers = GenerateUniqueNumbers(12);

            // Generate 5 unique passwords
            string[] passwords = GenerateUniquePasswords(5);

            // Insert records into the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                for (int i = 0; i < 5000; i++)
                {
                    int cardNo = cardNumbers[i % 12];
                    string cardPassword = passwords[i % 5];
                    var cardsExist2 = _context.Cards.Where(r => r.CardNo == cardNo).ToList();

                    if (cardsExist2.Count == 0)
                    {
                        // SQL command to insert a record
                        string sql = "INSERT INTO Cards (Card_No, Card_Password, Card_Price, Card_Status, User_ID, User_Name, Number_Of_Subjects) " +
                                     "VALUES (@CardNo, @CardPassword, '10', 1, NULL, NULL, 1)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@CardNo", cardNo);
                            command.Parameters.AddWithValue("@CardPassword", cardPassword);
                            command.ExecuteNonQuery();
                        }
                    }

                }
                    connection.Close();
            }

        }

        private int[] GenerateUniqueNumbers(int count)
        {
            // Generate unique numbers logic
            Random random = new Random();
            HashSet<int> uniqueNumbers = new HashSet<int>();

            while (uniqueNumbers.Count < count)
            {
                int newNumber = random.Next(1000000, 99999999); // Generate a 12-digit number
                uniqueNumbers.Add(newNumber);
            }

            return uniqueNumbers.ToArray();
        }

        private string[] GenerateUniquePasswords(int count)
        {
            Random random = new Random();
            string[] passwords = new string[count];
            for (int i = 0; i < count; i++)
            {
                // Generate a random string with both digits and letters
                passwords[i] = GenerateRandomString(random, 5);
            }
            return passwords;
        }

        private string GenerateRandomString(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Include both digits and letters
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
