using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class LeaveController : Controller
        {
        private readonly SchoolContext _context;
        public LeaveController(SchoolContext context)
            {
            _context = context;

            }

        public async Task<IActionResult> Index()
            {
            var leaves = await _context.tblLeaves.ToListAsync();
            var faculties = await _context.tblFaculties.ToDictionaryAsync(c => c.FacultyID, c => c.FirstName);
            ViewBag.Faculties = faculties;
            return View(leaves);
            }
        public IActionResult Create()
            {
            //ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "StudentID");

            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName");
            ViewBag.SubjectID = new SelectList(_context.tblSubject, "SubjectID", "SubjectName");

            var leave = new Leave
                {
                StartDate = DateTime.Today, // Set StartDate to today's date
                EndDate = DateTime.Today,   // Set EndDate to today's date
                Status = "Pending" // Set initial status
                };

            return View(leave);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leave leave)
            {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                {
                ModelState.AddModelError("", "Student ID not found in session.");
                return View(leave);
                }


            leave.StudentID = studentId.Value;
            if (ModelState.IsValid)
                {
                leave.Status = "Pending";
                _context.tblLeaves.Add(leave);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Assessment created successfully!";

                return RedirectToAction(nameof(Index));
                }
            //ViewBag.StudentID = new SelectList(_context.tblStudent, "StudentID", "FirstName", leave.StudentID);
            ViewBag.FacultyID = new SelectList(await _context.tblFaculties.ToListAsync(), "FacultyID", "FirstName", leave.FacultyID);
            ViewBag.SubjectID = new SelectList(await _context.tblSubject.ToListAsync(), "SubjectID", "SubjectName", leave.SubjectID);

            return View(leave);
            }



        public async Task<IActionResult> FacultyIndex()
            {
            var facultyEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(facultyEmail))
                {
                return RedirectToAction("Login", "Account");
                }

            var faculty = await _context.tblFaculties.FirstOrDefaultAsync(f => f.Email == facultyEmail);
            if (faculty == null)
                {
                return RedirectToAction("Login", "Account");
                }

            var leaves = await _context.tblLeaves.Where(l => l.FacultyID == faculty.FacultyID).ToListAsync();
            var faculties = await _context.tblStudent.ToDictionaryAsync(c => c.StudentID, c => c.FirstName);
            ViewBag.Faculties = faculties;
            return View(leaves);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int LeaveID, string Status)
            {
            var leave = await _context.tblLeaves.FindAsync(LeaveID);
            if (leave == null)
                {
                return NotFound();
                }

            leave.Status = Status;
            _context.Update(leave);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(FacultyIndex));
            }

        // GET: Leaves/Details/5
        public async Task<IActionResult> Details(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var leave = await _context.tblLeaves
                .FirstOrDefaultAsync(m => m.LeaveID == id);
            if (leave == null)
                {
                return NotFound();
                }

            return View(leave);
            }


        // GET: Leaves/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var leave = await _context.tblLeaves.FindAsync(id);
            if (leave == null)
                {
                return NotFound();
                }
            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName", leave.FacultyID);
            ViewBag.SubjectID = new SelectList(_context.tblSubject, "SubjectID", "SubjectName", leave.SubjectID);
            return View(leave);
            }

        // POST: Leaves/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Leave leave)
            {
            if (id != leave.LeaveID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    _context.Update(leave);
                    await _context.SaveChangesAsync();
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!LeaveExists(leave.LeaveID))
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
            ViewBag.FacultyID = new SelectList(_context.tblFaculties, "FacultyID", "FirstName", leave.FacultyID);
            ViewBag.SubjectID = new SelectList(_context.tblSubject, "SubjectID", "SubjectName", leave.SubjectID);
            return View(leave);
            }
        // GET: Leaves/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var leave = await _context.tblLeaves
                .FirstOrDefaultAsync(m => m.LeaveID == id);
            if (leave == null)
                {
                return NotFound();
                }

            return View(leave);
            }

        // POST: Leaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var leave = await _context.tblLeaves.FindAsync(id);
            _context.tblLeaves.Remove(leave);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }

        private bool LeaveExists(int id)
            {
            return _context.tblLeaves.Any(e => e.LeaveID == id);
            }
        }
    }
