using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class CourseController : Controller
        {
        private readonly SchoolContext _context;

        public CourseController(SchoolContext context)
            {
            _context = context;
            }

        public async Task<IActionResult> Index()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var coursesQuery = _context.tblCourse.AsQueryable();



            // Filter courses based on the logged-in admin's InstituteID
            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    coursesQuery = coursesQuery.Where(c => c.InstituteID == instituteID.Value);
                    }
                }

            var courses = await coursesQuery.ToListAsync();
            var institutes = await _context.tblInstitute.ToDictionaryAsync(c => c.InstituteID, c => c.Name);
            ViewBag.Institutes = institutes;

            var years = await _context.tblCourse.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(courses);
            }

        // GET: Course/Create
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

            ViewBag.InstituteID = new SelectList(institutesQuery, "InstituteID", "Name");

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
        public async Task<IActionResult> Create(Course course)
            {
            if (ModelState.IsValid)
                {
                _context.Add(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course added successfully!";
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

            ViewBag.InstituteID = new SelectList(institutesQuery, "InstituteID", "Name", course.InstituteID);

            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges, course.Year);

            return View(course);
            }

        // GET: Course/Edit/5

        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var course = await _context.tblCourse.FindAsync(id);
            if (course == null)
                {
                return NotFound();
                }

            // Set ViewBag with relevant institutes
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    ViewBag.InstituteID = new SelectList(
                        _context.tblInstitute.Where(i => i.InstituteID == instituteID.Value),
                        "InstituteID", "Name", course.InstituteID);
                    }
                }
            else
                {
                ViewBag.InstituteID = new SelectList(
                    _context.tblInstitute,
                    "InstituteID", "Name", course.InstituteID);
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
            return View(course);
            }
        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
            {
            if (id != course.CourseID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    var existingCourse = await _context.tblCourse.FindAsync(id);
                    if (existingCourse != null)
                        {
                        // Update properties
                        existingCourse.Name = course.Name;
                        existingCourse.Code = course.Code;
                        existingCourse.InstituteID = course.InstituteID;
                        existingCourse.Year = course.Year;
                        existingCourse.ModifiedBy = DateTime.Now;

                        _context.Update(existingCourse);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Course updated successfully!";
                        }
                    else
                        {
                        return NotFound();
                        }
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!CourseExists(course.CourseID))
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

            // Reload institutes list
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    ViewBag.InstituteID = new SelectList(
                        _context.tblInstitute.Where(i => i.InstituteID == instituteID.Value),
                        "InstituteID", "Name", course.InstituteID);
                    }
                }
            else
                {
                ViewBag.InstituteID = new SelectList(
                    _context.tblInstitute,
                    "InstituteID", "Name", course.InstituteID);
                }

            return View(course);
            }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var course = await _context.tblCourse
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
                {
                return NotFound();
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

            // Set ViewBag with relevant institutes for consistency
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    ViewBag.InstituteID = new SelectList(
                        _context.tblInstitute.Where(i => i.InstituteID == instituteID.Value),
                        "InstituteID", "Name", course.InstituteID);
                    }
                }
            else
                {
                ViewBag.InstituteID = new SelectList(
                    _context.tblInstitute,
                    "InstituteID", "Name", course.InstituteID);
                }

            return View(course);
            }


        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var course = await _context.tblCourse.FindAsync(id)
;
            if (course == null)
                {
                return NotFound();
                }
            _context.tblCourse.Remove(course);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Course deleted successfully!";

            return RedirectToAction(nameof(Index));
            }




        private bool CourseExists(int id)
            {
            return _context.tblCourse.Any(e => e.CourseID == id);
            }



        // GET: Course/FilterCourses
        public async Task<IActionResult> FilterCourses(string InstituteID, string Year)
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var coursesQuery = _context.tblCourse.AsQueryable();

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

            var courses = await coursesQuery.ToListAsync();
            var institutes = await _context.tblInstitute.ToDictionaryAsync(c => c.InstituteID, c => c.Name);
            ViewBag.Institutes = institutes;

            var years = await _context.tblCourse.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(courses);
            }

        }
    }
