using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;


namespace SIS_Dev.Controllers
    {
    public class TimetableController : Controller
        {
        private readonly SchoolContext _context;

        public TimetableController(SchoolContext context)
            {
            _context = context;
            }

        private async Task<bool> IsTimeSlotAvailable(Timetable timetable)
            {
            var overlappingTimetables = await _context.tblTimetable
                .Where(t => t.FacultyID == timetable.FacultyID
                            && t.Days == timetable.Days
                            && t.StartTime < timetable.EndTime
                            && t.EndTime > timetable.StartTime
                            && t.TimetableID != timetable.TimetableID) // Exclude the current timetable if editing
                .ToListAsync();

            return !overlappingTimetables.Any();
            }


        [HttpGet]
        public async Task<IActionResult> GetCourses(int instituteId)
            {
            var courses = await _context.tblCourse
                .Where(c => c.InstituteID == instituteId)
                .Select(c => new { c.CourseID, c.Name })
                .ToListAsync();

            return Json(courses);
            }

        [HttpGet]
        public async Task<IActionResult> GetStand(int courseId)
            {
            var standards = await _context.tblStandard
                .Where(s => s.CourseID == courseId)
                .Select(s => new { s.StandardID, s.StandardName })
                .ToListAsync();

            return Json(standards);
            }

        [HttpGet]
        public async Task<IActionResult> GetSubjects(int standardId)
            {
            var subjects = await _context.tblSubject
                .Where(s => s.StandardID == standardId)
                .Select(s => new { s.SubjectID, s.SubjectName })
                .ToListAsync();

            return Json(subjects);
            }

        [HttpGet]
        public async Task<IActionResult> GetFaculties(int subjectId)
            {
            var faculties = await _context.tblFaculties
                .Where(f => f.SubjectID == subjectId)
                .Select(f => new { f.FacultyID, f.FirstName })
                .ToListAsync();

            return Json(faculties);
            }


        // GET: Timetable/Create
        public async Task<IActionResult> Create()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userRole == "Admin")
                {
                // Get the admin details based on the logged-in user's email
                var admin = await _context.tblAdmin.FirstOrDefaultAsync(a => a.Email == userEmail);

                if (admin != null)
                    {
                    var instituteID = admin.InstituteID;

                    // Filter dropdowns by the logged-in admin's institute
                    ViewData["Institutes"] = new SelectList(await _context.tblInstitute.Where(i => i.InstituteID == instituteID).ToListAsync(), "InstituteID", "Name");
                    ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == instituteID).ToListAsync(), "CourseID", "Name");
                    ViewData["Standards"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewData["Subjects"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewData["Faculties"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    }
                }
            else
                {
                // SuperAdmin or other roles, show all options
                ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name");
                ViewData["Courses"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["Standards"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["Subjects"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["Faculties"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year++)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewData["Years"] = new SelectList(yearRanges);

            return View();
            }

        // POST: Timetable/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Timetable timetable)
            {
            if (ModelState.IsValid)
                {
                if (await IsTimeSlotAvailable(timetable))
                    {
                    timetable.CreatedBy = DateTime.Now;
                    _context.Add(timetable);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                    }
                else
                    {
                    ModelState.AddModelError("", "The selected time slot is not available for the faculty member.");
                    }
                }

            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userRole == "Admin")
                {
                var admin = await _context.tblAdmin.FirstOrDefaultAsync(a => a.Email == userEmail);

                if (admin != null)
                    {
                    var instituteID = admin.InstituteID;

                    // Filter dropdowns by the logged-in admin's institute
                    ViewData["Institutes"] = new SelectList(await _context.tblInstitute.Where(i => i.InstituteID == instituteID).ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                    ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == instituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                    ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                    ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                    ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                    }
                }
            else
                {
                // SuperAdmin or other roles
                ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == timetable.InstituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                }

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year++)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewData["Years"] = new SelectList(yearRanges, timetable.Year);

            return View(timetable);
            }



        // Other methods...
        // GET: Timetable/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var timetable = await _context.tblTimetable.FindAsync(id);
            if (timetable == null)
                {
                return NotFound();
                }



            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userRole == "Admin")
                {
                var admin = await _context.tblAdmin.FirstOrDefaultAsync(a => a.Email == userEmail);

                if (admin != null)
                    {
                    var instituteID = admin.InstituteID;

                    // Filter dropdowns by the logged-in admin's institute
                    ViewData["Institutes"] = new SelectList(await _context.tblInstitute.Where(i => i.InstituteID == instituteID).ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                    ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == instituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                    ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                    ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                    ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                    }
                }
            else
                {
                // SuperAdmin or other roles
                ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == timetable.InstituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                }

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year++)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewData["Years"] = new SelectList(yearRanges);


            return View(timetable);
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Timetable timetable, int id)
            {
            if (id != timetable.TimetableID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                if (await IsTimeSlotAvailable(timetable))
                    {
                    try
                        {
                        _context.Update(timetable);
                        await _context.SaveChangesAsync();
                        }
                    catch (DbUpdateConcurrencyException)
                        {
                        if (!TimetableExists(timetable.TimetableID))
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
                else
                    {
                    ModelState.AddModelError("", "The selected time slot is not available for the faculty member.");
                    }
                }

            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userRole == "Admin")
                {
                var admin = await _context.tblAdmin.FirstOrDefaultAsync(a => a.Email == userEmail);

                if (admin != null)
                    {
                    var instituteID = admin.InstituteID;

                    // Filter dropdowns by the logged-in admin's institute
                    ViewData["Institutes"] = new SelectList(await _context.tblInstitute.Where(i => i.InstituteID == instituteID).ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                    ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == instituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                    ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                    ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                    ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                    }
                }
            else
                {
                // SuperAdmin or other roles
                ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == timetable.InstituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                }

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year++)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewData["Years"] = new SelectList(yearRanges);


            return View(timetable);
            }

        private bool TimetableExists(int id)
            {
            return _context.tblTimetable.Any(e => e.TimetableID == id);
            }

        // GET: Timetable/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var timetable = await _context.tblTimetable
                .FirstOrDefaultAsync(m => m.TimetableID == id);
            if (timetable == null)
                {
                return NotFound();
                }
            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userRole == "Admin")
                {
                var admin = await _context.tblAdmin.FirstOrDefaultAsync(a => a.Email == userEmail);

                if (admin != null)
                    {
                    var instituteID = admin.InstituteID;

                    // Filter dropdowns by the logged-in admin's institute
                    ViewData["Institutes"] = new SelectList(await _context.tblInstitute.Where(i => i.InstituteID == instituteID).ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                    ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == instituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                    ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                    ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                    ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                    }
                }
            else
                {
                // SuperAdmin or other roles
                ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);
                ViewData["Courses"] = new SelectList(await _context.tblCourse.Where(c => c.InstituteID == timetable.InstituteID).ToListAsync(), "CourseID", "Name", timetable.CourseID);
                ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
                ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
                ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
                }
            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year++)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewData["Years"] = new SelectList(yearRanges);


            return View(timetable);
            }



        // POST: Timetable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var timetable = await _context.tblTimetable.FindAsync(id);
            if (timetable == null)
                {
                return NotFound();
                }

            _context.tblTimetable.Remove(timetable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }

        public async Task<IActionResult> Index()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var instituteID = HttpContext.Session.GetInt32("InstituteID");

            IQueryable<Course> courses = _context.tblCourse.AsQueryable();
            IQueryable<Standard> standards = _context.tblStandard.AsQueryable();
            IQueryable<Timetable> timetables = _context.tblTimetable.AsQueryable();

            if (userRole == "Admin")
                {
                if (instituteID.HasValue)
                    {
                    courses = courses.Where(c => c.InstituteID == instituteID.Value);
                    standards = standards.Where(s => s.InstituteID == instituteID.Value);
                    timetables = timetables.Where(t => t.InstituteID == instituteID.Value);
                    }
                }

            var timetableList = await (
                from t in timetables
                join c in _context.tblCourse on t.CourseID equals c.CourseID
                join f in _context.tblFaculties on t.FacultyID equals f.FacultyID
                join s in _context.tblSubject on t.SubjectID equals s.SubjectID
                join st in _context.tblStandard on t.StandardID equals st.StandardID
                join i in _context.tblInstitute on t.InstituteID equals i.InstituteID
                select new TimetableViewModel
                    {
                    TimetableID = t.TimetableID,
                    CourseID = t.CourseID,
                    CourseName = c.Name,
                    FacultyID = t.FacultyID,
                    FacultyName = f.FirstName,
                    SubjectID = t.SubjectID,
                    SubjectName = s.SubjectName,
                    StandardName = st.StandardName,
                    Days = t.Days,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    InstituteID = t.InstituteID,
                    Name = i.Name,
                    Year = t.Year,
                    }).ToListAsync();

            ViewData["Courses"] = new SelectList(await courses.ToListAsync(), "CourseID", "Name");
            ViewData["Standards"] = new List<SelectListItem>(); // Initialize empty list for standards
            ViewData["Years"] = await _context.tblTimetable.Select(c => c.Year).Distinct().ToListAsync();

            return View(timetableList);
            }

        [HttpGet]
        public async Task<JsonResult> GetStandards(int courseId)
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var instituteID = HttpContext.Session.GetInt32("InstituteID");

            IQueryable<Standard> standards = _context.tblStandard.Where(s => s.CourseID == courseId);

            if (userRole == "Admin" && instituteID.HasValue)
                {
                standards = standards.Where(s => s.InstituteID == instituteID.Value);
                }

            var standardsList = await standards
                .Select(s => new SelectListItem
                    {
                    Value = s.StandardID.ToString(),
                    Text = s.StandardName
                    }).ToListAsync();

            return Json(standardsList);
            }


        public async Task<IActionResult> Display(int courseId, string standardName, int standardid)
            {
            var timetables = await (
                from t in _context.tblTimetable
                join c in _context.tblCourse on t.CourseID equals c.CourseID
                join f in _context.tblFaculties on t.FacultyID equals f.FacultyID
                join s in _context.tblSubject on t.SubjectID equals s.SubjectID
                join st in _context.tblStandard on t.StandardID equals st.StandardID
                where t.CourseID == courseId && t.StandardID == standardid
                select new TimetableViewModel
                    {
                    TimetableID = t.TimetableID,
                    CourseID = t.CourseID,
                    CourseName = c.Name,
                    FacultyID = t.FacultyID,
                    FacultyName = f.FirstName,
                    SubjectID = t.SubjectID,
                    SubjectName = s.SubjectName,
                    StandardID = t.StandardID,
                    StandardName = st.StandardName,
                    Days = t.Days,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime
                    }).ToListAsync();
            var selectedCourse = await _context.tblCourse.FindAsync(courseId);
            ViewBag.SelectedCourse = selectedCourse?.Name;
            ViewBag.SelectedStandard = standardName;

            return View(timetables);
            }

        }
    }
