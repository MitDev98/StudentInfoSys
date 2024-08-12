using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class AssessmentController : Controller
        {
        private readonly SchoolContext _context;

        public AssessmentController(SchoolContext context)
            {
            _context = context;
            }

        public JsonResult GetStandards(int courseId)
            {
            var standards = _context.tblStandard
                                    .Where(s => s.CourseID == courseId)
                                    .Select(s => new { s.StandardID, s.StandardName })
                                    .ToList();
            return Json(standards);
            }

        //public JsonResult GetSubjects(int courseId, string standardName)
        //    {
        //    var subjects = _context.tblSubject
        //                           .Where(s => s.CourseID == courseId && s.StandardID == standardName)
        //                           .Select(s => new { s.SubjectID, s.SubjectName })
        //                           .ToList();
        //    return Json(subjects);
        //    }

        public IActionResult Create()
            {
            ViewBag.SubjectName = new SelectList(_context.tblSubject, "SubjectName", "SubjectName");
            ViewBag.StandardName = new SelectList(_context.tblStandard, "StandardName", "StandardName");
            ViewBag.CourseID = new SelectList(_context.tblCourse, "CourseID", "Name");
            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName");
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name");

            return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assessment assessment)
            {

            if (_context.tblAssessment.Any(c => c.SubjectName == assessment.SubjectName && c.FacultyID == assessment.FacultyID))
                {
                ModelState.AddModelError(string.Empty, "A course with the same name or code already exists.");
                }

            if (ModelState.IsValid)
                {
                assessment.CreatedBy = DateTime.Now;
                _context.tblAssessment.Add(assessment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Assessment created successfully!";

                return RedirectToAction(nameof(Index));
                }
            ViewBag.SubjectName = new SelectList(_context.tblSubject, "SubjectName", "SubjectName", assessment.SubjectName);
            ViewBag.StandardName = new SelectList(_context.tblStandard, "StandardName", "StandardName", assessment.StandardID);
            ViewBag.CourseID = new SelectList(_context.tblCourse, "CourseID", "Name", assessment.CourseID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName", assessment.FacultyID);
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", assessment.InstituteID);

            return View(assessment);
            }


        // GET: Assessment/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var assessment = await _context.tblAssessment.FindAsync(id);
            if (assessment == null)
                {
                return NotFound();
                }
            ViewBag.SubjectName = new SelectList(_context.tblSubject, "SubjectName", "SubjectName", assessment.SubjectName);
            ViewBag.StandardName = new SelectList(_context.tblStandard, "StandardName", "StandardName", assessment.StandardID);
            ViewBag.CourseID = new SelectList(_context.tblCourse, "CourseID", "Name", assessment.CourseID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName", assessment.FacultyID);
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", assessment.InstituteID);

            return View(assessment);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assessment assessment)
            {
            if (id != assessment.AssessmentID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    assessment.ModifiedBy = DateTime.Now;
                    _context.tblAssessment.Update(assessment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Assessment updated successfully!";
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!AssessmentExists(assessment.AssessmentID))
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
            ViewBag.SubjectName = new SelectList(_context.tblSubject, "SubjectName", "SubjectName", assessment.SubjectName);
            ViewBag.StandardName = new SelectList(_context.tblStandard, "StandardName", "StandardName", assessment.StandardID);
            ViewBag.CourseID = new SelectList(_context.tblCourse, "CourseID", "Name", assessment.CourseID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName", assessment.FacultyID);
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", assessment.InstituteID);

            return View(assessment);
            }


        //delete


        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var assessment = await _context.tblAssessment
                .FirstOrDefaultAsync(m => m.AssessmentID == id);
            if (assessment == null)
                {
                return NotFound();
                }
            ViewBag.SubjectName = new SelectList(_context.tblSubject, "SubjectName", "SubjectName", assessment.SubjectName);
            ViewBag.StandardName = new SelectList(_context.tblStandard, "StandardName", "StandardName", assessment.StandardID);
            ViewBag.CourseID = new SelectList(_context.tblCourse, "CourseID", "Name", assessment.CourseID);
            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName", assessment.FacultyID);
            return View(assessment);
            }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {

            var assessment = await _context.tblAssessment.FindAsync(id);
            if (assessment == null)
                {
                return NotFound();
                }
            //// Manually delete related subjects
            var assessmentdetails = _context.tblAssessmentResult.Where(s => s.AssessmentID == id);
            _context.tblAssessmentResult.RemoveRange(assessmentdetails);

            _context.tblAssessment.Remove(assessment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Assessment deleted successfully!";
            return RedirectToAction(nameof(Index));

            }



        public async Task<IActionResult> Index()
            {
            var userRole = HttpContext.Session.GetString("UserRole");
            // var assessments = _context.tblAssessment.Include(a => a.InstituteID).AsQueryable();
            var assessments = _context.tblAssessment.AsQueryable();

            if (userRole == "Admin1" || userRole == "Admin2" || userRole == "Admin3")
                {
                var instituteID = HttpContext.Session.GetInt32("InstituteID");
                if (instituteID.HasValue)
                    {
                    assessments = assessments.Where(a => a.InstituteID == instituteID.Value);
                    }
                }

            var courses = await _context.tblCourse.ToDictionaryAsync(c => c.CourseID, c => c.Name);
            ViewBag.Courses = courses;
            var faculties = await _context.tblFaculties.ToDictionaryAsync(f => f.FacultyID, f => f.FirstName);
            ViewBag.Faculties = faculties;
            var institutes = await _context.tblInstitute.ToDictionaryAsync(i => i.InstituteID, i => i.Name);
            ViewBag.Institutes = institutes;

            return View(await assessments.ToListAsync());
            }




        private bool AssessmentExists(int id)
            {
            return _context.tblAssessment.Any(e => e.AssessmentID == id);
            }
        }
    }