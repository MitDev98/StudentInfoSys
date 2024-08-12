using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class AssessmentResultController : Controller
        {
        private readonly SchoolContext _context;

        public AssessmentResultController(SchoolContext context)
            {
            _context = context;
            }

        //public async Task<IActionResult> Index()
        //    {

        //    var assessmentDetails = await _context.tblAssessmentResult.ToListAsync();
        //    var assessments = await _context.tblAssessment.ToDictionaryAsync(c => c.AssessmentID, c => c.SubjectName);
        //    ViewBag.Assessments = assessments;
        //    var students = await _context.tblStudent.ToDictionaryAsync(c => c.StudentID, c => c.FirstName);
        //    ViewBag.Students = students;
        //    return View(assessmentDetails);
        //    }


        //public async Task<IActionResult> Index()
        //    {
        //    var userRole = HttpContext.Session.GetString("UserRole");
        //    var userFacultyID = HttpContext.Session.GetInt32("FacultyID");

        //    var assessmentResults = _context.tblAssessmentResult.AsQueryable();

        //    if (userRole == "Admin1" || userRole == "Admin2" || userRole == "Admin3")
        //        {
        //        var instituteID = HttpContext.Session.GetInt32("InstituteID");

        //        if (instituteID.HasValue)
        //            {
        //            assessmentResults = assessmentResults.Where(a => a.InstituteID == instituteID.Value);
        //            }
        //        }
        //    else if (userRole == "Faculty" && userFacultyID.HasValue)
        //        {
        //        assessmentResults = assessmentResults.Where(ar => ar.Assessment.FacultyID == userFacultyID.Value);
        //        }

        //    var assessments = await _context.tblAssessment.ToDictionaryAsync(a => a.AssessmentID, a => a.SubjectName);
        //    ViewBag.Assessments = assessments;
        //    var students = await _context.tblStudent.ToDictionaryAsync(s => s.StudentID, s => s.FirstName);
        //    ViewBag.Students = students;
        //    var institutes = await _context.tblInstitute.ToDictionaryAsync(i => i.InstituteID, i => i.Name);
        //    ViewBag.Institutes = institutes;

        //    return View(await assessmentResults.ToListAsync());
        //    }




        //public async Task<IActionResult> Index()
        //    {
        //    var userRole = HttpContext.Session.GetString("UserRole");
        //    var userInstituteID = HttpContext.Session.GetInt32("InstituteID");
        //    var userFacultyID = HttpContext.Session.GetInt32("FacultyID");

        //    var assessmentResults = from ar in _context.tblAssessmentResult
        //                            join a in _context.tblAssessment on ar.AssessmentID equals a.AssessmentID
        //                            where (userRole == "Admin1" || userRole == "Admin2" || userRole == "Admin3") && userInstituteID.HasValue
        //                            ? a.InstituteID == userInstituteID.Value
        //                            : userRole == "Faculty" && userFacultyID.HasValue
        //                            ? a.FacultyID == userFacultyID.Value
        //                            : true
        //                            select new
        //                                {
        //                                ar,
        //                                a.SubjectName,
        //                                a.InstituteID
        //                                };

        //    var result = await assessmentResults.ToListAsync();
        //    var assessments = result.Select(r => new
        //        {
        //        r.ar.AssessmentID,
        //        r.SubjectName
        //        }).ToDictionary(a => a.AssessmentID, a => a.SubjectName);

        //    var students = await _context.tblStudent.ToDictionaryAsync(s => s.StudentID, s => s.FirstName);
        //    var institutes = await _context.tblInstitute.ToDictionaryAsync(i => i.InstituteID, i => i.Name);

        //    ViewBag.Assessments = assessments;
        //    ViewBag.Students = students;
        //    ViewBag.Institutes = institutes;

        //    return View(result.Select(r => r.ar));
        //    }


        public IActionResult Index()
            {
            var assessmentResults = _context.tblAssessmentResult.ToList();
            var students = _context.tblStudent.ToList();

            var viewModel = new AssessmentResultsViewModel
                {
                AssessmentResults = assessmentResults,
                Students = students
                };

            var institutes = _context.tblInstitute
                                     .ToDictionary(i => i.InstituteID, i => i.Name);

            ViewBag.Institutes = institutes;

            return View(viewModel);
            }









        public IActionResult Create()
            {
            ViewBag.AssessmentID = new SelectList(_context.tblAssessment, "AssessmentID", "SubjectName");
            ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "FirstName");
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name");

            return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssessmentResult assessmentDetails)
            {
            if (ModelState.IsValid)
                {
                assessmentDetails.CreatedBy = DateTime.Now;
                _context.tblAssessmentResult.Add(assessmentDetails);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "AssessmentResult inserted successfully!";

                return RedirectToAction(nameof(Index));
                }
            ViewBag.AssessmentID = new SelectList(_context.tblAssessment, "AssessmentID", "SubjectName", assessmentDetails.AssessmentID);
            ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "FirstName", assessmentDetails.StudentID);
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", assessmentDetails.InstituteID);

            return View(assessmentDetails);
            }


        // GET: Assessment/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", assessmentDetails.InstituteID);

            return View(assessmentDetails);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AssessmentResult assessmentDetails)
            {
            if (id != assessmentDetails.AssessmentResultID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    assessmentDetails.ModifiedBy = DateTime.Now;
                    _context.tblAssessmentResult.Update(assessmentDetails);
                    TempData["SuccessMessage"] = "AssessmentResult updated successfully!";

                    await _context.SaveChangesAsync();
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!AssessmentExists(assessmentDetails.AssessmentResultID))
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
            ViewBag.AssessmentID = new SelectList(_context.tblAssessment, "AssessmentID", "SubjectName", assessmentDetails.AssessmentID);
            ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "FirstName", assessmentDetails.StudentID);
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", assessmentDetails.InstituteID);

            return View(assessmentDetails);
            }

        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var assessmentDetails = await _context.tblAssessmentResult
                .FirstOrDefaultAsync(m => m.AssessmentResultID == id);

            if (assessmentDetails == null)
                {
                return NotFound();
                }

            ViewBag.AssessmentID = new SelectList(_context.tblAssessment, "AssessmentID", "SubjectName", assessmentDetails.AssessmentID);
            ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "FirstName", assessmentDetails.StudentID);
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", assessmentDetails.InstituteID);

            return View(assessmentDetails);
            }

        // POST: AssessmentResult/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var assessmentDetails = await _context.tblAssessmentResult.FindAsync(id);

            if (assessmentDetails == null)
                {
                return NotFound();
                }

            _context.tblAssessmentResult.Remove(assessmentDetails);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "AssessmentResult removed successfully!";

            return RedirectToAction(nameof(Index));
            }


        private bool AssessmentExists(int assessmentID)
            {
            throw new NotImplementedException();
            }
        }
    }
