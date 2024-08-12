using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class DepartmentController : Controller
        {
        private readonly SchoolContext _context;

        public DepartmentController(SchoolContext context)
            {
            _context = context;
            }

        // GET: Courses
        public async Task<IActionResult> Index()
            {
            return View(await _context.tblDepartment.ToListAsync());
            }

        // GET: Courses/Create
        public IActionResult Create()
            {
            return View();
            }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
            {
            if (_context.tblDepartment.Any(c => c.DName == department.DName && c.Code == department.Code))
                {
                ModelState.AddModelError(string.Empty, "A Department with the same name or code already exists.");
                }

            if (ModelState.IsValid)
                {
                department.CreatedBy = DateTime.Now;
                _context.tblDepartment.Add(department);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Department Inserted successfully!";

                return RedirectToAction(nameof(Index));
                }
            return View(department);
            }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var department = await _context.tblDepartment.FindAsync(id);
            if (department == null)
                {
                return NotFound();
                }
            return View(department);
            }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
            {
            if (id != department.DID)
                {
                return NotFound();
                }

            if (_context.tblDepartment.Any(c => (c.DName == department.DName || c.Code == department.Code) && c.DID != id))
                {
                ModelState.AddModelError(string.Empty, "A Department with the same name or code already exists.");
                }
            if (ModelState.IsValid)
                {
                try
                    {
                    department.ModifiedBy = DateTime.Now;
                    _context.tblDepartment.Update(department);
                    TempData["SuccessMessage"] = "Department updated successfully!";

                    await _context.SaveChangesAsync();
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!CourseExists(department.DID))
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
            return View(department);
            }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var department = await _context.tblDepartment
                .FirstOrDefaultAsync(m => m.DID == id);
            if (department == null)
                {
                return NotFound();
                }

            return View(department);
            }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var department = await _context.tblDepartment.FindAsync(id);
            _context.tblDepartment.Remove(department);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Department removed successfully!";

            return RedirectToAction(nameof(Index));
            }

        private bool CourseExists(int id)
            {
            return _context.tblDepartment.Any(e => e.DID == id);
            }
        }
    }
