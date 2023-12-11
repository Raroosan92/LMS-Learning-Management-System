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
            var card = GetCards_And_Details();
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName");
            return View();
            //return View();
        }
        public List<SelectListItem> _Classes { get; set; }
        public List<SelectListItem> _Subjecs { get; set; }

        List<string> _ClassesLst = new List<string>();
        List<string> _SubjecsLst = new List<string>();


        // POST: Cards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CardNo,CardPassword,CardPrice,CardStatus,UserId,UserName")] Card card)
        {
            if (ModelState.IsValid)
            {
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
                        CardSubjects = new CardSubject
                        {
                            CardNo = card.Id,
                            SubjectId = int.Parse(_SubjecsLst[b]),
                            ClassId = int.Parse(_ClassesLst[a])
                        };
                        _context.CardSubjects.Add(CardSubjects);
                        await _context.SaveChangesAsync();
                    }
                }



                _context.Add(card);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Descriptions", card.ClassId);
            //ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Abbreviation", card.SubjectId);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,CardNo,CardPassword,CardPrice,CardStatus,UserId,UserName")] Card card)
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


                var stList = _context.Cards.Include(c => c.User).ToList();
                for (int i = 0; i < stList.Count; i++)
                {
                    //var subjects = _context.Subjects.Where(r => r.Id == stList[i].SubjectId).SingleOrDefault();
                    //var classes = _context.Classes.Where(r => r.Id == stList[i].ClassId).SingleOrDefault();
                    var users = _context.AspNetUsers.Where(r => r.Id == stList[i].UserId).SingleOrDefault();
                    //stList[i].Classdesc = classes.Descriptions;
                    //stList[i].Subjectdesc = subjects.Name;
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
            catch (Exception dd)
            {
                return new JsonResult("Error");
            }

        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (LMSContext db = new LMSContext())
            {
                //CardSubject cardsdtl = db.CardSubjects.Where(x => x.CardNo == id).FirstOrDefault<CardSubject>();
                //db.CardSubjects.Remove(cardsdtl);
                //db.SaveChanges();



                Card cards = db.Cards.Where(x => x.Id == id).FirstOrDefault<Card>();
                db.Cards.Remove(cards);
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



        public Mixed_Cards_CardDetails GetCards_And_Details()
        {


            List<CardSubject> query = (from d in _context.CardSubjects
                                       join h in _context.Cards on d.CardNo equals h.Id
                                       select d).ToList();

            var model = new Mixed_Cards_CardDetails()
            {
                HD_Collection = _context.Cards.ToList(),
                DTL_Collection = query.AsEnumerable(),
                Subject_Collection=_context.Subjects.ToList(),
                Class_Collection=_context.Classes.ToList()
            };
            return (model);

        }
    }
}
