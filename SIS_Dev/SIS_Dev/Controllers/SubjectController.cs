using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class SubjectController : Controller
        {
        private readonly SchoolContext _context;

        public SubjectController(SchoolContext context)
            {
            _context = context;
            }


        public JsonResult GetCourses(int instituteId)
            {
            var courses = _context.tblCourse.Where(c => c.InstituteID == instituteId).Select(c => new { c.CourseID, c.Name }).ToList();
            return Json(courses);
            }
        public JsonResult GetStandards(int courseId)
            {
            var standards = _context.tblStandard
                .Where(s => s.CourseID == courseId)
                .Select(s => new { standardID = s.StandardID, name = s.StandardName })
                .ToList();

            return Json(standards);
            }

        // GET: Subjects
        public async Task<IActionResult> Index()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            IQueryable<Subject> subjects = _context.tblSubject;

            if (userRole == "Admin")
                {
                var admin = await _context.tblAdmin.FirstOrDefaultAsync(a => a.Email == userEmail);
                if (admin != null)
                    {
                    var instituteId = admin.InstituteID;
                    subjects = subjects.Where(s => s.InstituteID == instituteId);
                    }
                }

            var subjectsList = await subjects.ToListAsync();
            var institutes = await _context.tblInstitute.ToDictionaryAsync(i => i.InstituteID, i => i.Name);
            ViewBag.Institutes = institutes;

            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;

            var standards = await _context.tblStandard.ToDictionaryAsync(s => s.StandardID, s => s.StandardName);
            ViewBag.Standards = standards;

            var years = await _context.tblSubject.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(subjectsList);
            }



        // GET: Subjects/Create
        public IActionResult Create()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            var admin = _context.tblAdmin.FirstOrDefault(a => a.Email == userEmail);

            if (admin != null && (userRole == "Admin"))
                {
                // For logged-in admin, filter by their institute
                var instituteID = admin.InstituteID;
                ViewBag.InstituteID = new SelectList(_context.tblInstitute.Where(i => i.InstituteID == instituteID), "InstituteID", "Name");
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == instituteID), "CourseID", "Name");
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => _context.tblCourse.Where(c => c.InstituteID == instituteID).Select(c => c.CourseID).Contains(s.CourseID)), "StandardID", "StandardName");
                }
            else
                {
                // SuperAdmin or other roles, show all options
                ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name");
                ViewBag.CourseID = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.StandardID = new SelectList(Enumerable.Empty<SelectListItem>());
                }

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);

            return View();
            }


        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
            {
            if (ModelState.IsValid)
                {
                subject.CreatedBy = DateTime.Now;
                _context.tblSubject.Add(subject);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Subject created successfully!";
                return RedirectToAction(nameof(Index));
                }

            var admin = _context.tblAdmin.FirstOrDefault(a => a.Email == HttpContext.Session.GetString("UserEmail"));

            if (admin != null && (HttpContext.Session.GetString("UserRole") == "Admin"))
                {
                var instituteID = admin.InstituteID;
                ViewBag.InstituteID = new SelectList(_context.tblInstitute.Where(i => i.InstituteID == instituteID), "InstituteID", "Name", subject.InstituteID);
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == instituteID), "CourseID", "Name", subject.CourseID);
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => _context.tblCourse.Where(c => c.InstituteID == instituteID).Select(c => c.CourseID).Contains(s.CourseID)), "StandardID", "StandardName", subject.StandardID);
                }
            else
                {
                ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", subject.InstituteID);
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == subject.InstituteID), "CourseID", "Name", subject.CourseID);
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == subject.CourseID), "StandardID", "StandardName", subject.StandardID);
                }

            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges, subject.Year);

            return View(subject);
            }


        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var subject = await _context.tblSubject
                .FirstOrDefaultAsync(m => m.SubjectID == id);
            if (subject == null)
                {
                return NotFound();
                }
            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var admin = _context.tblAdmin.FirstOrDefault(a => a.Email == userEmail);

            if (admin != null && (userRole == "Admin"))
                {
                // For logged-in admin, filter by their institute
                var instituteID = admin.InstituteID;
                ViewBag.InstituteID = new SelectList(_context.tblInstitute.Where(i => i.InstituteID == instituteID), "InstituteID", "Name");
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == instituteID), "CourseID", "Name");
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => _context.tblCourse.Where(c => c.InstituteID == instituteID).Select(c => c.CourseID).Contains(s.CourseID)), "StandardID", "StandardName");
                }
            else
                {
                // SuperAdmin or other roles, show all options
                ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name");
                ViewBag.CourseID = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.StandardID = new SelectList(Enumerable.Empty<SelectListItem>());
                }



            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);

            return View(subject);
            }

        // GET: Faculty/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var subject = await _context.tblSubject.FindAsync(id);
            if (subject == null)
                {
                return NotFound();
                }

            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var admin = _context.tblAdmin.FirstOrDefault(a => a.Email == userEmail);

            if (admin != null && (userRole == "Admin"))
                {
                // For logged-in admin, filter by their institute
                var instituteID = admin.InstituteID;
                ViewBag.InstituteID = new SelectList(_context.tblInstitute.Where(i => i.InstituteID == instituteID), "InstituteID", "Name", subject.InstituteID);
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == instituteID), "CourseID", "Name", subject.CourseID);
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => _context.tblCourse.Where(c => c.InstituteID == instituteID).Select(c => c.CourseID).Contains(s.CourseID)), "StandardID", "StandardName", subject.StandardID);
                }
            else
                {
                // SuperAdmin or other roles, show all options
                ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", subject.InstituteID);
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == subject.InstituteID), "CourseID", "Name", subject.CourseID);
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == subject.CourseID), "StandardID", "StandardName", subject.StandardID);
                }

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);

            return View(subject);
            }

        // POST: Faculty/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject subject)
            {
            if (id != subject.SubjectID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    subject.ModifiedBy = DateTime.Now;
                    _context.tblSubject.Update(subject);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Faculty updated successfully!";
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!SubjectExists(subject.SubjectID))
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

            var admin = _context.tblAdmin.FirstOrDefault(a => a.Email == HttpContext.Session.GetString("UserEmail"));

            if (admin != null && (HttpContext.Session.GetString("UserRole") == "Admin"))
                {
                var instituteID = admin.InstituteID;
                ViewBag.InstituteID = new SelectList(_context.tblInstitute.Where(i => i.InstituteID == instituteID), "InstituteID", "Name", subject.InstituteID);
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == instituteID), "CourseID", "Name", subject.CourseID);
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => _context.tblCourse.Where(c => c.InstituteID == instituteID).Select(c => c.CourseID).Contains(s.CourseID)), "StandardID", "StandardName", subject.StandardID);
                }
            else
                {
                ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", subject.InstituteID);
                ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == subject.InstituteID), "CourseID", "Name", subject.CourseID);
                ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == subject.CourseID), "StandardID", "StandardName", subject.StandardID);
                }

            return View(subject);
            }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var subject = await _context.tblSubject.FindAsync(id);
            if (subject == null)
                {
                return NotFound();
                }

            _context.tblSubject.Remove(subject);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Subject deleted successfully!";
            return RedirectToAction(nameof(Index));
            }

        private bool SubjectExists(int id)
            {
            return _context.tblSubject.Any(e => e.SubjectID == id);
            }

        // GET: Course/FilterCourses
        public async Task<IActionResult> FilterSubject(string InstituteID, string Year)
            {

            var userRole = HttpContext.Session.GetString("UserRole");
            var coursesQuery = _context.tblSubject.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteIDString = HttpContext.Session.GetString("InstituteID");
                if (int.TryParse(instituteIDString, out int instituteID))
                    {
                    coursesQuery = coursesQuery.Where(c => c.InstituteID == instituteID);
                    }
                }

            if (!string.IsNullOrEmpty(InstituteID))
                {
                if (int.TryParse(InstituteID, out int instituteID))
                    {
                    coursesQuery = coursesQuery.Where(c => c.InstituteID == instituteID);
                    }
                }

            if (!string.IsNullOrEmpty(Year))
                {
                coursesQuery = coursesQuery.Where(c => c.Year == Year);
                }

            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;

            var standards = await _context.tblStandard.ToDictionaryAsync(s => s.StandardID, s => s.StandardName);
            ViewBag.Standards = standards;


            //var courses = await coursesQuery.ToListAsync();
            var institutes = await _context.tblInstitute.ToDictionaryAsync(c => c.InstituteID, c => c.Name);
            ViewBag.Institutes = institutes;

            var years = await _context.tblSubject.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(await coursesQuery.ToListAsync());
            }





        }
    }
