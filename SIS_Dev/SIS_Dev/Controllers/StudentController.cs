using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class StudentController : Controller
        {
        private readonly SchoolContext _context;

        public StudentController(SchoolContext context)
            {
            _context = context;
            }

        // Action to display assessment results for logged-in student
        public async Task<IActionResult> StudentDashboard()
            {
            var currentUserEmail = HttpContext.Session.GetString("UserEmail");
            var studentId = await _context.tblStudent.FirstOrDefaultAsync(f => f.Email == currentUserEmail);

            if (studentId == null)
                {
                return RedirectToAction("Login", "Account");
                }

            var assessmentResults = await _context.tblAssessmentResult
                .Where(ar => ar.StudentID == studentId.StudentID)
                .ToListAsync();

            //fetch assessment to pass view 
            var assessments = await _context.tblAssessment.ToListAsync();
            ViewData["Assessments"] = assessments;

            var assessmentdate = await _context.tblAssessment.ToListAsync();
            ViewData["AssessmentsDate"] = assessmentdate;

            return View(assessmentResults);

            }

        // GET: /Student/GetCourses
        public JsonResult GetCourses(int instituteId)
            {
            var courses = _context.tblCourse
                .Where(c => c.InstituteID == instituteId)
                .Select(c => new { c.CourseID, c.Name })
                .ToList();

            return Json(courses);
            }

        // GET: /Student/GetStandards
        public JsonResult GetStandards(int courseId)
            {
            var standards = _context.tblStandard
                .Where(s => s.CourseID == courseId)
                .Select(s => new { s.StandardID, s.StandardName })
                .ToList();

            return Json(standards);
            }

        // GET: /Student/GetFaculties
        public JsonResult GetFaculties(int standardId)
            {
            var faculties = _context.tblFaculties
                .Where(f => f.StandardID == standardId)
                .Select(f => new { f.FacultyID, f.FirstName })
                .ToList();

            return Json(faculties);
            }

        public async Task<IActionResult> Index()
            {
            var userRole = HttpContext.Session.GetString("UserRole");

            var studentsQuery = _context.tblStudent.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");

                if (instituteID.HasValue)
                    {
                    studentsQuery = studentsQuery.Where(s => s.InstituteID == instituteID.Value);
                    }
                }

            //var students = await studentsQuery.ToListAsync();

            // Handle null values and provide default empty dictionaries if necessary
            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;

            var institutes = await _context.tblInstitute.ToDictionaryAsync(i => i.InstituteID, i => i.Name);
            ViewBag.Institutes = institutes;

            var faculties = await _context.tblFaculties.ToDictionaryAsync(f => f.FacultyID, f => f.FirstName);
            ViewBag.Faculties = faculties;

            var standards = await _context.tblStandard.ToDictionaryAsync(s => s.StandardID, s => s.StandardName);
            ViewBag.Standards = standards;

            var years = await _context.tblCourse.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(await studentsQuery.ToListAsync());
            }


        // GET: Students/Create
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
            ViewBag.StandardID = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.FacultyID = new SelectList(Enumerable.Empty<SelectListItem>());
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

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
            {
            if (ModelState.IsValid)
                {
                student.CreatedBy = DateTime.Now;
                _context.tblStudent.Add(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student inserted successfully!";
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


            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", student.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == student.InstituteID), "CourseID", "Name", student.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == student.CourseID), "StandardID", "StandardName", student.StandardID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties.Where(f => f.StandardID == student.StandardID), "FacultyID", "FirstName", student.FacultyID);
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges, student.Year);
            return View(student);
            }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var student = await _context.tblStudent.FindAsync(id);
            if (student == null)
                {
                return NotFound();
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", student.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == student.InstituteID), "CourseID", "Name", student.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == student.CourseID), "StandardID", "StandardName", student.StandardID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties.Where(f => f.StandardID == student.StandardID), "FacultyID", "FirstName", student.FacultyID);

            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);

            return View(student);
            }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
            {
            if (id != student.StudentID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    student.ModifiedBy = DateTime.Now;
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Student updated successfully!";
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!StudentExists(student.StudentID))
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
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", student.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == student.InstituteID), "CourseID", "Name", student.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == student.CourseID), "StandardID", "StandardName", student.StandardID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties.Where(f => f.StandardID == student.StandardID), "FacultyID", "FirstName", student.FacultyID);

            return View(student);
            }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var student = await _context.tblStudent
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
                {
                return NotFound();
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", student.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == student.InstituteID), "CourseID", "Name", student.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == student.CourseID), "StandardID", "StandardName", student.StandardID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties.Where(f => f.StandardID == student.StandardID), "FacultyID", "FirstName", student.FacultyID);

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);
            return View(student);
            }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
            {
            var student = await _context.tblStudent.FindAsync(id);
            _context.tblStudent.Remove(student);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student removed successfully!";

            return RedirectToAction(nameof(Index));
            }

        private bool StudentExists(int id)
            {
            return _context.tblStudent.Any(e => e.StudentID == id);
            }


        // GET: Course/FilterCourses
        public async Task<IActionResult> FilterStudent(string InstituteID, string Year)
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var coursesQuery = _context.tblStudent.AsQueryable();

            if (userRole == "Admin1" || userRole == "Admin2" || userRole == "Admin3")
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

            var faculties = await _context.tblFaculties.ToDictionaryAsync(f => f.FacultyID, f => f.FirstName);
            ViewBag.Faculties = faculties;

            var standards = await _context.tblStandard.ToDictionaryAsync(s => s.StandardID, s => s.StandardName);
            ViewBag.Standards = standards;

            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;

            // var courses = await coursesQuery.ToListAsync();
            var institutes = await _context.tblInstitute.ToDictionaryAsync(c => c.InstituteID, c => c.Name);
            ViewBag.Institutes = institutes;

            var years = await _context.tblStudent.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(await coursesQuery.ToListAsync());
            //return View(courses);
            }



        }
    }
