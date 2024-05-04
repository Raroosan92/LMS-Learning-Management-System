using LMS_Learning_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace LMS_Learning_Management_System.Controllers
{
    [Authorize(Roles = "admin")]

    public class TeacherCardsSalesController : Controller
    {

        private Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        public AppIdentityDbContext _contextUsers = new AppIdentityDbContext();
        private readonly IConfiguration _configuration;
        private readonly LMSContext _context;
        string time;
        string Year;
        string Month;
        DateTime Jor;

        public TeacherCardsSalesController(LMSContext context, Microsoft.AspNetCore.Identity.UserManager<AppUser> userMgr, AppIdentityDbContext iden, IConfiguration configuration)
        {
            _context = context;
            userManager = userMgr;
            _configuration = configuration;
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
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Abbreviation", "Abbreviation", "-- أختر --");
            ViewData["TeacherId"] = new SelectList(_context.VTeacherSalesCards.Select(r => new { r.TeacherName, r.TeacherUserID }).Distinct(), "TeacherName", "TeacherName", "-- أختر --");


            var model = new Mixed_VTeacher_Sales_Cards()
            {

                Teacher_Sales_Cards_Collection = await _context.VTeacherSalesCards.OrderByDescending(r => r.UserName).ToListAsync()

            };
            GetUserRole();
            return View(model);
            //return View(await _context.Classes.OrderByDescending(r=>r.Id).ToListAsync());
        }

        [HttpPost]
        public IActionResult GetData()
        {
            try
            {

                DateTime? DateFrom = new DateTime();
                DateTime? DateTo = new DateTime();

                string df = Request.Form["startDate"];
                if (!string.IsNullOrEmpty(df))
                {
                    DateFrom = DateTime.Parse(df);
                }
                else
                {
                    //string ff = SqlDateTime.MinValue.Value.ToShortDateString();
                    DateFrom = null;

                }
                string dt = Request.Form["endDate"];
                if (!string.IsNullOrEmpty(dt))
                {
                    DateTo = DateTime.Parse(dt);

                }
                else
                {
                    //string cc = SqlDateTime.MaxValue.Value.ToShortDateString();
                    DateTo = null;

                }


                string UserID = Request.Form["teacherName"].First();
                if (UserID.Length == 0 || UserID.Length == null)
                {
                    UserID = null;
                }
                string SubjectID = Request.Form["subject1"].First();
                if (SubjectID.Length == 0 || SubjectID.Length == null)
                {
                    SubjectID = null;
                }

                if (User.IsInRole("admin"))
                {

                    try
                    {



                        SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));



                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Get_Teachers_Sales_SP";


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Date_From", DateFrom);
                        cmd.Parameters.AddWithValue("@Date_To", DateTo);
                        cmd.Parameters.AddWithValue("@teacherID", UserID);
                        cmd.Parameters.AddWithValue("@SubjectId", SubjectID);
                        cmd.Parameters.AddWithValue("@check", "S");

                        con.Open();

                        List<TeacherSalesCard> responseData = new List<TeacherSalesCard>();
                        Mixed_VTeacher_Sales_Cards model = new Mixed_VTeacher_Sales_Cards();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TeacherSalesCard dataModel = new TeacherSalesCard();

                                try
                                {
                                    dataModel.CardNo = int.Parse(reader["Card_No"].ToString());
                                    dataModel.CardPrice = reader.IsDBNull(reader.GetOrdinal("Card_Price"))
                                        ? null
                                        : reader["Card_Price"].ToString();

                                    dataModel.NumberOfSubjects = int.TryParse(reader["Number_Of_Subjects"].ToString(), out int subjects)
                                        ? subjects
                                        : (int?)null;

                                    dataModel.TeacherCardPrice = decimal.TryParse(reader["Teacher_Card_Price"].ToString(), out decimal teacherCardPrice)
                                        ? teacherCardPrice
                                        : (decimal?)null;

                                    dataModel.CenterCardPrice = decimal.TryParse(reader["Center_Card_Price"].ToString(), out decimal centerCardPrice)
                                        ? centerCardPrice
                                        : (decimal?)null;

                                    dataModel.UserName = reader["UserName"].ToString();
                                    dataModel.UserTypeDesc = reader["UserTypeDesc"].ToString();
                                    dataModel.TeacherName = reader["Teacher_Name"].ToString();
                                    dataModel.Subject = reader["Subject"].ToString();
                                    dataModel.Class = reader["Class"].ToString();
                                    dataModel.Country = reader["Country"].ToString();
                                    dataModel.TeacherId = reader["Teacher_ID"].ToString();

                                    dataModel.PaymentAmount = decimal.TryParse(reader["Payment_Amount"].ToString(), out decimal paymentAmount)
                                        ? paymentAmount
                                        : (decimal?)null;

                                    dataModel.IsPayment = bool.TryParse(reader["Is_Payment"].ToString(), out bool isPayment)
                                        ? isPayment
                                        : (bool?)null;

                                    dataModel.StudentName = reader["Student_Name"].ToString();

                                    dataModel.CreatedDate = DateTime.TryParse(reader["Created_Date"].ToString(), out DateTime createdDate)
                                        ? createdDate
                                        : default;

                                    dataModel.PaymentDate = DateTime.TryParse(reader["Payment_Date"].ToString(), out DateTime paymentDate)
                                        ? (DateTime?)paymentDate
                                        : null;

                                    dataModel.CardSer = int.TryParse(reader["CardSer"].ToString(), out int cardSer)
                                        ? cardSer
                                        : default;

                                    dataModel.TeacherUserID = reader["Teacher_ID"].ToString();

                                    dataModel.Semester = int.TryParse(reader["Semester"].ToString(), out int semester)
                                        ? semester
                                        : default;

                                    // Add the dataModel to your responseData list or process it as needed
                                }
                                catch (Exception ex)
                                {

                                }

                                responseData.Add(dataModel);

                                 model = new Mixed_VTeacher_Sales_Cards()
                                {

                                    Teacher_Sales_Cards_Collection = responseData.AsEnumerable()

                                };
                            }
                            con.Close();
                        }
                        ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(r => r.Status == true), "Abbreviation", "Abbreviation", "-- أختر --");
                        ViewData["TeacherId"] = new SelectList(_context.VTeacherSalesCards.Select(r => new { r.TeacherName, r.TeacherUserID }), "TeacherName", "TeacherName", "-- أختر --");

                        GetUserRole();
                        return View("index", model);
                    }
                    catch (Exception ex)
                    {

                        return Json(new { success = false, message = "حدث خطأ أثناء محاولة البحث" });
                    }


                }
                else
                {
                    return new JsonResult(new { data = "" });

                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { data = ex.Message.ToString() });

            }
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

        public void GetTime()
        {

            TimeZoneInfo AST = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");
            DateTime utc = DateTime.UtcNow;
            Jor = TimeZoneInfo.ConvertTimeFromUtc(utc, AST);
            time = Jor.ToString();
            Year = Jor.Year.ToString();
            Month = Jor.Month.ToString();

        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Pay(List<int> cardno, List<int> CardSer, List<decimal> amount)
        {
            if (ModelState.IsValid)
            {
                GetTime();

                // Iterate through the selected rows and update their payment details
                for (int i = 0; i < cardno.Count; i++)
                {
                    var cardNo = cardno[i];
                    var cardSer = CardSer[i];
                    var paymentAmount = amount[i];

                    var cardDetails = _context.CardSubjects.FirstOrDefault(r => r.Id == cardSer);

                    if (cardDetails != null && cardDetails.TeacherId != null)
                    {
                        // Update the payment details
                        cardDetails.PaymentAmount = paymentAmount;
                        cardDetails.IsPayment = true;
                        cardDetails.PaymentDate = Jor;

                        // Exclude specific properties from modification
                        _context.Entry(cardDetails).Property(x => x.CardNo).IsModified = false;
                        _context.Entry(cardDetails).Property(x => x.SubjectId).IsModified = false;
                        _context.Entry(cardDetails).Property(x => x.ClassId).IsModified = false;
                        _context.Entry(cardDetails).Property(x => x.TeacherId).IsModified = false;
                        _context.Entry(cardDetails).Property(x => x.Semester).IsModified = false;
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Reload the page or return appropriate response
                // Redirect to the action or view you want after processing
                var model = new Mixed_VTeacher_Sales_Cards()
                {

                    Teacher_Sales_Cards_Collection = await _context.VTeacherSalesCards.OrderByDescending(r => r.UserName).ToListAsync()

                };
                GetUserRole();

                return View("index", model);

            }
            var model2 = _context.VTeacherSalesCards.OrderByDescending(r => r.UserName);
            GetUserRole();

            return View("index", model2.ToList());
        }


        //public async Task<IActionResult> Pay(int cardno, int CardSer, decimal amount)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        GetTime();

        //        var Card_Details = _context.CardSubjects.Where(r => r.Id == CardSer).FirstOrDefault();

        //        if (Card_Details.TeacherId != null)
        //        {
        //            // Update the properties directly on the tracked entity
        //            Card_Details.PaymentAmount = amount;
        //            Card_Details.IsPayment = true;
        //            Card_Details.PaymentDate = Jor;

        //            // Exclude specific properties from modification
        //            _context.Entry(Card_Details).Property(x => x.CardNo).IsModified = false;
        //            _context.Entry(Card_Details).Property(x => x.SubjectId).IsModified = false;
        //            _context.Entry(Card_Details).Property(x => x.ClassId).IsModified = false;
        //            _context.Entry(Card_Details).Property(x => x.TeacherId).IsModified = false;
        //            _context.Entry(Card_Details).Property(x => x.Semester).IsModified = false;
        //            await _context.SaveChangesAsync();
        //        }

        //        var model = new Mixed_VTeacher_Sales_Cards()
        //        {

        //            Teacher_Sales_Cards_Collection = await _context.VTeacherSalesCards.OrderByDescending(r => r.UserName).ToListAsync()

        //        };
        //        GetUserRole();
        //        return View(model);

        //        //return View("index", model);

        //    }
        //    var model2 = _context.VTeacherSalesCards.OrderByDescending(r => r.UserName);
        //    GetUserRole();
        //    return View(model2.ToList());

        //    //return View("index", model2.ToList());

        //}
    }
}
