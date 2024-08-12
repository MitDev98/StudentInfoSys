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
            // Populate dropdown lists for the create view
            ViewData["Courses"] = new SelectList(await _context.tblCourse.ToListAsync(), "CourseID", "Name");
            ViewData["Faculties"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["Subjects"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["Standards"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name");

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
                    _context.Add(timetable);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                    }
                else
                    {
                    ModelState.AddModelError("", "The selected time slot is not available for the faculty member.");
                    }
                }

            // Re-populate dropdown lists in case of validation failure
            ViewData["Courses"] = new SelectList(await _context.tblCourse.ToListAsync(), "CourseID", "Name", timetable.CourseID);
            ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
            ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.CourseID == timetable.CourseID && s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
            ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
            ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);


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

            // Populate dropdowns
            ViewData["Courses"] = new SelectList(await _context.tblCourse.ToListAsync(), "CourseID", "Name", timetable.CourseID);
            ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
            ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.CourseID == timetable.CourseID && s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
            ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
            ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);


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

            ViewData["Courses"] = new SelectList(await _context.tblCourse.ToListAsync(), "CourseID", "Name", timetable.CourseID);
            ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
            ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.CourseID == timetable.CourseID && s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
            ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
            ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);


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
            // If ModelState is not valid, repopulate dropdowns and return to view
            ViewData["Courses"] = new SelectList(await _context.tblCourse.ToListAsync(), "CourseID", "Name", timetable.CourseID);
            ViewData["Faculties"] = new SelectList(await _context.tblFaculties.Where(f => f.SubjectID == timetable.SubjectID).ToListAsync(), "FacultyID", "FirstName", timetable.FacultyID);
            ViewData["Subjects"] = new SelectList(await _context.tblSubject.Where(s => s.CourseID == timetable.CourseID && s.StandardID == timetable.StandardID).ToListAsync(), "SubjectID", "SubjectName", timetable.SubjectID);
            ViewData["Standards"] = new SelectList(await _context.tblStandard.Where(s => s.CourseID == timetable.CourseID).ToListAsync(), "StandardID", "StandardName", timetable.StandardID);
            ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name", timetable.InstituteID);

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
            IQueryable<Timetable> timetables = _context.tblTimetable.AsQueryable();

            if (userRole == "Admin")
                {
                if (instituteID.HasValue)
                    {
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

                    }).ToListAsync();

            ViewData["Courses"] = new SelectList(await _context.tblCourse.ToListAsync(), "CourseID", "Name");
            ViewData["Standards"] = new List<SelectListItem>(); // Initialize empty list for standards
            ViewData["Institutes"] = new SelectList(await _context.tblInstitute.ToListAsync(), "InstituteID", "Name");

            return View(timetableList);
            }


        [HttpGet]
        public JsonResult GetStandards(int courseId)
            {
            var standards = _context.tblStandard
                .Where(s => s.CourseID == courseId) // Ensure tblStandard has CourseID
                .Select(s => new { value = s.StandardID, text = s.StandardName }) // Ensure StandardName is used as value
                .ToList();

            return Json(standards);
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
