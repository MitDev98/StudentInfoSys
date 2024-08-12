using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class FacultyController : Controller
        {
        private readonly SchoolContext _context;

        public FacultyController(SchoolContext context)
            {
            _context = context;
            }

        public async Task<IActionResult> Index()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var faculties = _context.tblFaculties.AsQueryable();

            if (userRole == "Admin")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    faculties = faculties.Where(f => f.InstituteID == instituteID.Value);
                    }
                }

            var institutes = await _context.tblInstitute.ToDictionaryAsync(i => i.InstituteID, i => i.Name);
            ViewBag.Institutes = institutes;

            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;

            var standards = await _context.tblStandard.ToDictionaryAsync(s => s.StandardID, s => s.StandardName);
            ViewBag.Standards = standards;

            var subjects = await _context.tblSubject.ToDictionaryAsync(s => s.SubjectID, s => s.SubjectName);
            ViewBag.Subjects = subjects;

            var years = await _context.tblFaculties.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(await faculties.ToListAsync());
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
            ViewBag.StandardID = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.SubjectID = new SelectList(Enumerable.Empty<SelectListItem>());
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
        public async Task<IActionResult> Create(Faculty faculty)
            {
            if (_context.tblFaculties.Any(c => c.Email == faculty.Email || c.Contactno == faculty.Contactno))
                {
                ModelState.AddModelError(string.Empty, "A Faculty with same email and contactno already exists.");
                }



            if (ModelState.IsValid)
                {
                faculty.CreatedBy = DateTime.Now;
                _context.tblFaculties.Add(faculty);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Faculty inserted successfully!";
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
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", faculty.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == faculty.InstituteID), "CourseID", "Name", faculty.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == faculty.CourseID), "StandardID", "StandardName", faculty.StandardID);
            ViewBag.SubjectID = new SelectList(_context.tblSubject.Where(s => s.StandardID == faculty.StandardID), "SubjectID", "SubjectName", faculty.SubjectID);
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges, faculty.Year);
            return View(faculty);
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
        [HttpGet]
        public IActionResult GetSubjects(int standardId)
            {
            var subjects = _context.tblSubject
                .Where(s => s.StandardID == standardId)
                .Select(s => new { s.SubjectID, s.SubjectName })
                .ToList();

            return Json(subjects);
            }

        // GET: Faculty/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var faculty = await _context.tblFaculties.FindAsync(id);
            if (faculty == null)
                {
                return NotFound();
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", faculty.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == faculty.InstituteID), "CourseID", "Name", faculty.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == faculty.CourseID), "StandardID", "StandardName", faculty.StandardID);
            ViewBag.SubjectID = new SelectList(_context.tblSubject.Where(s => s.StandardID == faculty.StandardID), "SubjectID", "SubjectName", faculty.SubjectID);
            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);

            return View(faculty);
            }

        // POST: Faculty/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Faculty faculty)
            {
            if (id != faculty.FacultyID)
                {
                return NotFound();
                }

            if (_context.tblFaculties.Any(c => (c.Email == faculty.Email || c.Contactno == faculty.Contactno) && c.FacultyID != id))
                {
                ModelState.AddModelError(string.Empty, "A Faculty with the same email or contactno already exists.");
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    faculty.ModifiedBy = DateTime.Now;
                    _context.tblFaculties.Update(faculty);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Faculty updated successfully!";

                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!FacultyExists(faculty.FacultyID))
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
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", faculty.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == faculty.InstituteID), "CourseID", "Name", faculty.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == faculty.CourseID), "StandardID", "StandardName", faculty.StandardID);
            ViewBag.SubjectID = new SelectList(_context.tblSubject.Where(s => s.StandardID == faculty.StandardID), "SubjectID", "SubjectName", faculty.SubjectID);
            return View(faculty);
            }

        // GET: Faculty/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var faculty = await _context.tblFaculties

                .FirstOrDefaultAsync(m => m.FacultyID == id);
            if (faculty == null)
                {
                return NotFound();
                }

            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", faculty.InstituteID);
            ViewBag.CourseID = new SelectList(_context.tblCourse.Where(c => c.InstituteID == faculty.InstituteID), "CourseID", "Name", faculty.CourseID);
            ViewBag.StandardID = new SelectList(_context.tblStandard.Where(s => s.CourseID == faculty.CourseID), "StandardID", "StandardName", faculty.StandardID);
            ViewBag.SubjectID = new SelectList(_context.tblSubject.Where(s => s.StandardID == faculty.StandardID), "SubjectID", "SubjectName", faculty.SubjectID);

            // Generate year ranges from 2000 to the current year
            var currentYear = DateTime.Now.Year;
            var yearRanges = new List<string>();
            for (int year = 2000; year <= currentYear; year += 1)
                {
                var endYear = Math.Min(year + 1, currentYear);
                yearRanges.Add($"{year}-{endYear}");
                }
            ViewBag.Years = new SelectList(yearRanges);

            return View(faculty);
            }

        // POST: Faculty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var faculty = await _context.tblFaculties.FindAsync(id);
            if (faculty == null)
                {
                return NotFound();
                }
            _context.tblFaculties.Remove(faculty);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Faculty removed successfully!";

            return RedirectToAction(nameof(Index));
            }

        private bool FacultyExists(int id)
            {
            return _context.tblFaculties.Any(e => e.FacultyID == id);
            }
        private bool AssessmentResultExists(int assessmentID)
            {
            throw new NotImplementedException();
            }

        // GET: Course/FilterCourses
        public async Task<IActionResult> FilterFaculty(string InstituteID, string Year)
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            var coursesQuery = _context.tblFaculties.AsQueryable();

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

            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;

            var standards = await _context.tblStandard.ToDictionaryAsync(s => s.StandardID, s => s.StandardName);
            ViewBag.Standards = standards;

            var subjects = await _context.tblSubject.ToDictionaryAsync(s => s.SubjectID, s => s.SubjectName);
            ViewBag.Subjects = subjects;

            //var courses = await coursesQuery.ToListAsync();
            var institutes = await _context.tblInstitute.ToDictionaryAsync(c => c.InstituteID, c => c.Name);
            ViewBag.Institutes = institutes;

            var years = await _context.tblFaculties.Select(c => c.Year).Distinct().ToListAsync();
            ViewBag.Years = years;

            return View(await coursesQuery.ToListAsync());
            }

        // Assessment

        public async Task<IActionResult> FacultyDashboardAssessment()
            {
            var currentUserEmail = HttpContext.Session.GetString("UserEmail");
            var faculty = await _context.tblFaculties.FirstOrDefaultAsync(f => f.Email == currentUserEmail);

            if (faculty == null)
                {
                return RedirectToAction("Login", "Account");
                }

            // Fetch assessments assigned to this faculty
            var assessments = await _context.tblAssessment
                .Where(a => a.FacultyID == faculty.FacultyID)
                .ToListAsync();

            // Fetch courses to pass to the view
            var courses = await _context.tblCourse.ToListAsync();

            ViewData["Courses"] = courses; // Pass courses to view

            return View(assessments);
            }



        //public async Task<IActionResult> StudentListAssessment(int assessmentId)
        //    {
        //    var assessment = await _context.tblAssessment.FindAsync(assessmentId);
        //    if (assessment == null)
        //        {
        //        return NotFound();
        //        }

        //    // Check if assessment results already exist
        //    var existingResults = await _context.tblAssessmentResult
        //        .Where(ar => ar.AssessmentID == assessmentId)
        //        .ToListAsync();

        //    if (existingResults.Count > 0)
        //        {
        //        // Redirect to ViewAssessmentResults if results already exist
        //        return RedirectToAction("ViewAssessmentResults", new { assessmentId });
        //        }

        //    //var students = await _context.tblStudent
        //    //    .Where(s => s.StandardID == assessment.StandardName)
        //    //    .ToListAsync();

        //    var model = new StudentListViewModel
        //        {
        //        AssessmentId = assessmentId,
        //        Students = students
        //        };

        //    return View(model);
        //    }



        [HttpPost]
        public async Task<IActionResult> SubmitAssessmentResults(StudentListViewModel model)
            {
            if (ModelState.IsValid)
                {
                foreach (var result in model.AssessmentResults)
                    {
                    result.CreatedBy = DateTime.UtcNow;

                    if (result.Absent)
                        {
                        result.ObtainedMarks = 0; // Default value to represent absence
                        result.Remark = "Absent"; // Default value to represent absence
                        }
                    else
                        {
                        // If Remark is empty, set it to an empty string instead of null
                        if (string.IsNullOrEmpty(result.Remark))
                            {
                            result.Remark = ""; // Set to empty string
                            }
                        }

                    var existingResult = await _context.tblAssessmentResult
                        .FirstOrDefaultAsync(ar => ar.AssessmentID == result.AssessmentID && ar.StudentID == result.StudentID);

                    if (existingResult == null)
                        {
                        _context.tblAssessmentResult.Add(result);
                        }
                    else
                        {
                        existingResult.ObtainedMarks = result.ObtainedMarks;
                        existingResult.Remark = result.Remark;
                        existingResult.Absent = result.Absent;
                        existingResult.CreatedBy = result.CreatedBy;
                        _context.tblAssessmentResult.Update(existingResult);
                        }
                    }

                await _context.SaveChangesAsync();
                return RedirectToAction("ViewAssessmentResults", new { assessmentId = model.AssessmentId });
                }

            var assessment = await _context.tblAssessment.FindAsync(model.AssessmentId);
            if (assessment != null)
                {
                model.Students = await _context.tblStudent
                    .Where(s => s.StandardID == assessment.StandardID)
                    .ToListAsync();
                }

            return View("StudentListAssessment", model);
            }


        public async Task<IActionResult> ViewAssessmentResults(int assessmentId)
            {
            var assessment = await _context.tblAssessment.FindAsync(assessmentId);
            if (assessment == null)
                {
                return NotFound();
                }

            var assessmentResults = await _context.tblAssessmentResult
                .Where(ar => ar.AssessmentID == assessmentId)
                .ToListAsync();

            var students = await _context.tblStudent
                .Where(s => s.StandardID == assessment.StandardID) // Adjust filtering if necessary
                .ToListAsync();

            var model = new AssessmentResultsViewModel
                {
                AssessmentId = assessmentId,
                AssessmentResults = assessmentResults,
                Students = students
                };

            return View(model);
            }
        public async Task<IActionResult> ViewAssessmentStatistics(int assessmentId)
            {
            var assessment = await _context.tblAssessment.FindAsync(assessmentId);
            if (assessment == null)
                {
                return NotFound();
                }

            var assessmentResults = await _context.tblAssessmentResult
                .Where(ar => ar.AssessmentID == assessmentId)
                .ToListAsync();

            var totalStudents = assessmentResults.Count;
            var absentStudents = assessmentResults.Count(ar => ar.Absent);
            var passedStudents = assessmentResults.Count(ar => !ar.Absent && ar.ObtainedMarks >= 40); // Adjust the pass mark as needed
            var failedStudents = totalStudents - passedStudents - absentStudents;

            // Calculate average marks
            double? averageMarksNullable = assessmentResults
                .Where(ar => !ar.Absent && ar.ObtainedMarks.HasValue) // Filter out absent students and null marks
                .Average(ar => ar.ObtainedMarks);

            // Convert nullable double? to double with a default value if null
            double averageMarks = averageMarksNullable ?? 0; // Use 0 or any default value you prefer

            var model = new AssessmentStatisticsViewModel
                {
                TotalStudents = totalStudents,
                PassedStudents = passedStudents,
                FailedStudents = failedStudents,
                AbsentStudents = absentStudents,
                AverageMarks = averageMarks
                };

            return View(model);
            }





        //Edit Assessment Result 

        public async Task<IActionResult> EditAssessmentResult(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var assessmentDetails = await _context.tblAssessmentResult.FindAsync(id);
            if (assessmentDetails == null)
                {
                return NotFound();
                }

            ViewBag.AssessmentID = new SelectList(_context.tblAssessment, "AssessmentID", "SubjectName", assessmentDetails.AssessmentID);
            ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "FirstName", assessmentDetails.StudentID);
            return View(assessmentDetails);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssessmentResult(int id, AssessmentResult result)
            {
            if (id != result.AssessmentResultID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    // Update ModifiedBy field
                    result.ModifiedBy = DateTime.UtcNow;
                    // Handle Remark field if empty
                    if (string.IsNullOrEmpty(result.Remark))
                        {
                        result.Remark = ""; // Set to empty string to avoid null value
                        }

                    _context.Update(result);
                    await _context.SaveChangesAsync();
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!AssessmentResultExists(result.AssessmentResultID))
                        {
                        return NotFound();
                        }
                    else
                        {
                        throw;
                        }
                    }
                return RedirectToAction(nameof(ViewAssessmentResults), new { assessmentId = result.AssessmentID });
                }

            ViewBag.AssessmentID = new SelectList(_context.tblAssessment, "AssessmentID", "SubjectName", result.AssessmentID);
            ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "FirstName", result.StudentID);
            return View(result);
            }


        //timetable and attendance


        public async Task<IActionResult> FacultyDashboard()
            {

            // Get current faculty's ID or email (based on your implementation)
            var currentUserEmail = HttpContext.Session.GetString("UserEmail");
            var faculty = await _context.tblFaculties.FirstOrDefaultAsync(f => f.Email == currentUserEmail);

            if (faculty == null)
                {
                // Handle error or redirect to login
                return RedirectToAction("Login", "Account");
                }

            // Fetch assessments assigned to this faculty
            var assessments = await _context.tblTimetable
                .Where(a => a.FacultyID == faculty.FacultyID)
                .ToListAsync();

            var courses = await _context.tblCourse.ToListAsync();
            var faculties = await _context.tblFaculties.ToListAsync();
            var subjects = await _context.tblSubject.ToListAsync();

            ViewData["Courses"] = courses;
            ViewData["Faculties"] = faculties;
            ViewData["Subjects"] = subjects;

            return View(assessments);
            }

        ////studentlist brbr krvu baki chhe
        //public async Task<IActionResult> StudentList(int courseId, string standardName, int subjectId)
        //    {

        //    var students = await _context.tblStudent
        //        .Where(s => s.CourseID == courseId && s.StandardID == standardName)
        //        .ToListAsync();

        //    var subject = await _context.tblSubject.FindAsync(subjectId);
        //    ViewData["SubjectName"] = subject?.SubjectName;

        //    return View(students);
        //    }

        public async Task<IActionResult> StudentList(int courseId, int standardId, int subjectId)
            {
            // Assuming standardId is passed as an integer now instead of string
            var students = await _context.tblStudent
                .Where(s => s.CourseID == courseId && s.StandardID == standardId)
                .ToListAsync();

            var subject = await _context.tblSubject.FindAsync(subjectId);
            ViewData["SubjectName"] = subject?.SubjectName;

            return View(students);
            }

        //[HttpPost]
        //public async Task<IActionResult> SaveAttendance(Dictionary<int, bool> Attendance, string SubjectName, int FacultyID)
        //    {
        //    var today = DateTime.Today;

        //    // Check if attendance already exists for today and this subject
        //    var existingAttendance = await _context.tblAttendence
        //        .Where(a => a.SubjectName == SubjectName && a.FacultyID == FacultyID && a.Date.Date == today)
        //        .ToListAsync();

        //    if (existingAttendance.Count > 0)
        //        {
        //        // Redirect to edit page with a message
        //        TempData["Message"] = "Attendance already taken for today.";
        //        return RedirectToAction("AbsentStudents", new { subjectName = SubjectName, facultyId = FacultyID });
        //        }

        //    foreach (var entry in Attendance)
        //        {
        //        var studentId = entry.Key;
        //        var isPresent = entry.Value;

        //        var attendance = new Attendance
        //            {
        //            Date = today,
        //            StartTime = new TimeSpan(9, 0, 0),  // Set your actual start time
        //            EndTime = new TimeSpan(10, 0, 0),   // Set your actual end time
        //            SubjectName = SubjectName,
        //            FacultyID = FacultyID,
        //            StudentID = studentId,
        //            Status = isPresent,
        //            Topic = "Topic of the day"  // Set your actual topic
        //            };

        //        _context.tblAttendence.Add(attendance);
        //        }

        //    await _context.SaveChangesAsync();

        //    // Redirect to the attendance list page
        //    return RedirectToAction("AbsentStudents", new { subjectName = SubjectName, facultyId = FacultyID });
        //    }

        public async Task<IActionResult> SaveAttendance(Dictionary<int, bool> Attendance, string SubjectName, int FacultyID)
            {
            var today = DateTime.Today;

            // Check if attendance already exists for today and this subject
            var existingAttendance = await _context.tblAttendence
                .Where(a => a.SubjectName == SubjectName && a.FacultyID == FacultyID && a.Date.Date == today)
                .ToListAsync();

            if (existingAttendance.Count > 0)
                {
                // Redirect to edit page with a message
                TempData["Message"] = "Attendance already taken for today.";
                return RedirectToAction("AbsentStudents", new { subjectName = SubjectName, facultyId = FacultyID });
                }

            foreach (var entry in Attendance)
                {
                var studentId = entry.Key;
                var isPresent = entry.Value;

                var attendance = new Attendance
                    {
                    Date = today,
                    StartTime = new TimeSpan(9, 0, 0),  // Set your actual start time
                    EndTime = new TimeSpan(10, 0, 0),   // Set your actual end time
                    SubjectName = SubjectName,
                    FacultyID = FacultyID,
                    StudentID = studentId,
                    Status = isPresent,
                    Topic = "Topic of the day"  // Set your actual topic
                    };

                _context.tblAttendence.Add(attendance);
                }

            await _context.SaveChangesAsync();

            // Redirect to the attendance list page
            return RedirectToAction("AbsentStudents", new { subjectName = SubjectName, facultyId = FacultyID });
            }


        //public async Task<IActionResult> AbsentStudents(string subjectName, int facultyId)
        //    {
        //    // Fetch attendance records for the given subject and faculty
        //    var attendanceRecords = await _context.tblAttendence
        //        .Where(a => a.SubjectName == subjectName && a.FacultyID == facultyId)
        //        .ToListAsync();

        //    // Create a list to hold view models for attendance records
        //    var attendanceViewModel = new List<AttendanceViewModel>();

        //    // Iterate through each attendance record
        //    foreach (var attendance in attendanceRecords)
        //        {
        //        // Fetch student details for the current attendance record
        //        var student = await _context.tblStudent
        //            .FirstOrDefaultAsync(s => s.StudentID == attendance.StudentID);

        //        // Ensure student is found before proceeding
        //        if (student != null)
        //            {
        //            // Create a view model instance and populate it
        //            var viewModel = new AttendanceViewModel
        //                {
        //                AttendanceID = attendance.AttendanceID,
        //                Date = attendance.Date,
        //                StartTime = attendance.StartTime,
        //                EndTime = attendance.EndTime,
        //                SubjectName = attendance.SubjectName,
        //                FacultyID = attendance.FacultyID,
        //                StudentID = attendance.StudentID,
        //                Status = attendance.Status,
        //                Topic = attendance.Topic,
        //                StudentFirstName = student.FirstName,
        //                StudentLastName = student.LastName,

        //                // Define the edit URL for each attendance record
        //                EditUrl = Url.Action("EditAttendance", "Faculty", new { attendanceId = attendance.AttendanceID })
        //                };

        //            // Add the view model to the list
        //            attendanceViewModel.Add(viewModel);
        //            }
        //        }

        //    // Pass the list of view models to the view
        //    ViewBag.Message = TempData["Message"];
        //    return View(attendanceViewModel);
        //    }

        public async Task<IActionResult> AbsentStudents(string subjectName, int facultyId)
            {
            // Fetch attendance records for the given subject and faculty
            var attendanceRecords = await _context.tblAttendence
                .Where(a => a.SubjectName == subjectName && a.FacultyID == facultyId)
                .ToListAsync();

            // Create a list to hold view models for attendance records
            var attendanceViewModel = new List<AttendanceViewModel>();

            // Iterate through each attendance record
            foreach (var attendance in attendanceRecords)
                {
                // Fetch student details for the current attendance record
                var student = await _context.tblStudent
                    .FirstOrDefaultAsync(s => s.StudentID == attendance.StudentID);

                // Ensure student is found before proceeding
                if (student != null)
                    {
                    // Create a view model instance and populate it
                    var viewModel = new AttendanceViewModel
                        {
                        AttendanceID = attendance.AttendanceID,
                        Date = attendance.Date,
                        StartTime = attendance.StartTime,
                        EndTime = attendance.EndTime,
                        SubjectName = attendance.SubjectName,
                        FacultyID = attendance.FacultyID,
                        StudentID = attendance.StudentID,
                        Status = attendance.Status,
                        Topic = attendance.Topic,
                        StudentFirstName = student.FirstName,
                        StudentLastName = student.LastName,

                        // Define the edit URL for each attendance record
                        EditUrl = Url.Action("EditAttendance", "Faculty", new { attendanceId = attendance.AttendanceID })
                        };

                    // Add the view model to the list
                    attendanceViewModel.Add(viewModel);
                    }
                }

            // Pass the list of view models to the view
            ViewBag.Message = TempData["Message"];
            return View(attendanceViewModel);
            }


        [HttpGet]
        public async Task<IActionResult> GetAttendanceDetails(int id)
            {
            var attendanceRecord = await _context.tblAttendence.FindAsync(id);

            if (attendanceRecord == null)
                {
                return NotFound();
                }

            var viewModel = new AttendanceViewModel
                {
                AttendanceID = attendanceRecord.AttendanceID,
                Status = attendanceRecord.Status
                };

            return Json(viewModel);
            }
        //public async Task<IActionResult> EditAttendance(int attendanceId)
        //    {
        //    var attendance = await _context.tblAttendence.FindAsync(attendanceId);

        //    if (attendance == null)
        //        {
        //        return NotFound();
        //        }

        //    // Fetch student details for the current attendance record
        //    var student = await _context.tblStudent
        //        .FirstOrDefaultAsync(s => s.StudentID == attendance.StudentID);

        //    if (student == null)
        //        {
        //        return NotFound();
        //        }

        //    var viewModel = new AttendanceViewModel
        //        {
        //        AttendanceID = attendance.AttendanceID,
        //        Date = attendance.Date,
        //        StartTime = attendance.StartTime,
        //        EndTime = attendance.EndTime,
        //        SubjectName = attendance.SubjectName,
        //        FacultyID = attendance.FacultyID,
        //        StudentID = attendance.StudentID,
        //        Status = attendance.Status,
        //        Topic = attendance.Topic,
        //        StudentFirstName = student.FirstName,
        //        StudentLastName = student.LastName
        //        };

        //    return View(viewModel);
        //    }



        //[HttpPost]
        //public async Task<IActionResult> EditAttendance(AttendanceViewModel model)
        //    {
        //    if (!ModelState.IsValid)
        //        {
        //        // Return the view with validation errors if model state is invalid
        //        return View(model);
        //        }

        //    try
        //        {
        //        // Find the attendance record in the database
        //        var attendance = await _context.tblAttendence.FindAsync(model.AttendanceID);

        //        if (attendance == null)
        //            {
        //            return NotFound();
        //            }

        //        // Update the attendance record with values from the view model
        //        attendance.Status = model.Status;

        //        // Save changes to the database
        //        _context.Update(attendance);
        //        await _context.SaveChangesAsync();

        //        TempData["Message"] = "Attendance updated successfully.";

        //        // Redirect to the attendance list action method
        //        return RedirectToAction("AbsentStudents", new { subjectName = model.SubjectName, facultyId = model.FacultyID });
        //        }
        //    catch (DbUpdateConcurrencyException)
        //        {
        //        ModelState.AddModelError("", "Concurrency error occurred while saving the attendance record.");
        //        return View(model);
        //        }
        //    }

        public async Task<IActionResult> EditAttendance(int attendanceId)
            {
            var attendance = await _context.tblAttendence.FindAsync(attendanceId);

            if (attendance == null)
                {
                return NotFound();
                }

            // Fetch student details for the current attendance record
            var student = await _context.tblStudent
                .FirstOrDefaultAsync(s => s.StudentID == attendance.StudentID);

            if (student == null)
                {
                return NotFound();
                }

            var viewModel = new AttendanceViewModel
                {
                AttendanceID = attendance.AttendanceID,
                Date = attendance.Date,
                StartTime = attendance.StartTime,
                EndTime = attendance.EndTime,
                SubjectName = attendance.SubjectName,
                FacultyID = attendance.FacultyID,
                StudentID = attendance.StudentID,
                Status = attendance.Status,
                Topic = attendance.Topic,
                StudentFirstName = student.FirstName,
                StudentLastName = student.LastName
                };

            return View(viewModel);
            }
        [HttpPost]
        public async Task<IActionResult> EditAttendance(AttendanceViewModel model)
            {
            if (!ModelState.IsValid)
                {
                // Return the view with validation errors if model state is invalid
                return View(model);
                }

            try
                {
                // Find the attendance record in the database
                var attendance = await _context.tblAttendence.FindAsync(model.AttendanceID);

                if (attendance == null)
                    {
                    return NotFound();
                    }

                // Update the attendance record with values from the view model
                attendance.Status = model.Status;

                // Save changes to the database
                _context.Update(attendance);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Attendance updated successfully.";

                // Redirect to the attendance list action method
                return RedirectToAction("AbsentStudents", new { subjectName = model.SubjectName, facultyId = model.FacultyID });
                }
            catch (DbUpdateConcurrencyException)
                {
                ModelState.AddModelError("", "Concurrency error occurred while saving the attendance record.");
                return View(model);
                }
            }




        public async Task<IActionResult> EditAttendanceSummary(string subjectName)
            {
            // Fetch attendance records for the given subject
            var attendanceRecords = await _context.tblAttendence
                .Where(a => a.SubjectName == subjectName)
                .ToListAsync();

            // Calculate total students, present, and absent counts
            int totalStudents = attendanceRecords.Select(a => a.StudentID).Distinct().Count();
            int presentCount = attendanceRecords.Count(a => a.Status);
            int absentCount = totalStudents - presentCount; // Assuming all students are listed and counted

            // Prepare a view model for the summary
            var attendanceSummaryViewModel = new AttendanceSummaryViewModel
                {
                SubjectName = subjectName,
                TotalStudents = totalStudents,
                PresentCount = presentCount,
                AbsentCount = absentCount
                };

            return View(attendanceSummaryViewModel);
            }





        }
    }
