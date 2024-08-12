using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class StandardController : Controller
        {
        private readonly SchoolContext _context;

        public StandardController(SchoolContext context)
            {
            _context = context;
            }

        public async Task<IActionResult> Index()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var standards = _context.tblStandard.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    standards = standards.Where(f => f.InstituteID == instituteID.Value);
                    }
                }

            var institutes = await _context.tblInstitute.ToDictionaryAsync(i => i.InstituteID, i => i.Name);
            ViewBag.Institutes = institutes;

            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;

            var years = await _context.tblStandard.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(await standards.ToListAsync());
            }



        // GET: Faculty/Create
        public IActionResult Create()
            {

            // Filter institutes based on user role
            var userRole = HttpContext.Session.GetString("UserRole");
            var institutesQuery = _context.tblInstitute.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    institutesQuery = institutesQuery.Where(c => c.InstituteID == instituteID.Value);
                    }
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name");
            ViewBag.CourseID = new SelectList(Enumerable.Empty<SelectListItem>());

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Standard standard)
            {


            if (ModelState.IsValid)
                {
                standard.CreatedBy = DateTime.Now;
                _context.tblStandard.Add(standard);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "standard inserted successfully!";
                return RedirectToAction(nameof(Index));
                }

            // If we got this far, something failed, redisplay the form
            var userRole = HttpContext.Session.GetString("UserRole");
            var institutesQuery = _context.tblInstitute.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    institutesQuery = institutesQuery.Where(c => c.InstituteID == instituteID.Value);
                    }
                }

            // Re-populate the dropdowns in case of a validation error
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", standard.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == standard.InstituteID), "CourseID", "Name", standard.CourseID);

            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges, standard.Year);
            return View(standard);
            }

        public JsonResult GetCourses(int instituteId)
            {
            var courses = _context.tblCourse.Where(c => c.InstituteID == instituteId).Select(c => new { c.CourseID, c.Name }).ToList();
            return Json(courses);
            }


        // GET: Faculty/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var standard = await _context.tblStandard.FindAsync(id);
            if (standard == null)
                {
                return NotFound();
                }
            // Filter institutes based on user role
            var userRole = HttpContext.Session.GetString("UserRole");
            var institutesQuery = _context.tblInstitute.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    institutesQuery = institutesQuery.Where(c => c.InstituteID == instituteID.Value);
                    }
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", standard.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == standard.InstituteID), "CourseID", "Name", standard.CourseID);

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);

            return View(standard);
            }

        // POST: Faculty/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Standard standard)
            {
            if (id != standard.StandardID)
                {
                return NotFound();
                }



            if (ModelState.IsValid)
                {
                try
                    {
                    standard.ModifiedBy = DateTime.Now;
                    _context.tblStandard.Update(standard);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Faculty updated successfully!";

                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!StandardExists(standard.StandardID))
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
            // Filter institutes based on user role
            var userRole = HttpContext.Session.GetString("UserRole");
            var institutesQuery = _context.tblInstitute.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    institutesQuery = institutesQuery.Where(c => c.InstituteID == instituteID.Value);
                    }
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", standard.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == standard.InstituteID), "CourseID", "Name", standard.CourseID);

            return View(standard);
            }


        // POST: Standard/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var standard = await _context.tblStandard.FindAsync(id);
            if (standard == null)
                {
                return NotFound();
                }
            // Filter institutes based on user role
            var userRole = HttpContext.Session.GetString("UserRole");
            var institutesQuery = _context.tblInstitute.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    institutesQuery = institutesQuery.Where(c => c.InstituteID == instituteID.Value);
                    }
                }

            _context.tblStandard.Remove(standard);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Standard deleted successfully!";
            return RedirectToAction(nameof(Index));
            }

        private bool StandardExists(int id)
            {
            return _context.tblStandard.Any(e => e.StandardID == id);
            }


        // GET: Course/FilterCourses
        public async Task<IActionResult> FilterStandard(string InstituteID, string Year)
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var coursesQuery = _context.tblStandard.AsQueryable();

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


            //var courses = await coursesQuery.ToListAsync();
            var institutes = await _context.tblInstitute.ToDictionaryAsync(c => c.InstituteID, c => c.Name);
            ViewBag.Institutes = institutes;

            var years = await _context.tblStandard.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(await coursesQuery.ToListAsync());
            }

        }
    }

