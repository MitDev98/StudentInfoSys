using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class InstituteController : Controller
        {
        private readonly SchoolContext _context;

        public InstituteController(SchoolContext context)
            {
            _context = context;
            }

        public async Task<IActionResult> Index()
            {
            // Fetching the list of institutes
            var institutes = await _context.tblInstitute.ToListAsync();

            // Pass the list of institutes to the view
            return View(institutes);
            }

        public IActionResult Create()
            {
            return View();
            }

        // POST: Institute/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Institute institute)
            {
            if (ModelState.IsValid)
                {
                _context.Add(institute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                }

            // If the model state is invalid, return the view with the same institute object
            return View(institute);
            }
        // GET: Institute/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var institute = await _context.tblInstitute.FindAsync(id);
            if (institute == null)
                {
                return NotFound();
                }

            ViewBag.AdminSelectList = new SelectList(_context.tblAdmin, "AdminID", "Email", institute.AdminID);
            return View(institute);
            }

        // POST: Institute/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Institute institute)
            {
            if (id != institute.InstituteID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    institute.ModifiedBy = DateTime.Now;
                    _context.tblInstitute.Update(institute);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Institute updated successfully!";
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!InstituteExists(institute.InstituteID))
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

            ViewBag.AdminSelectList = new SelectList(_context.tblAdmin, "AdminID", "Email", institute.AdminID);
            return View(institute);
            }

        // GET: Institute/Delete/5
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var institute = await _context.tblInstitute
                .FirstOrDefaultAsync(m => m.InstituteID == id);
            if (institute == null)
                {
                return NotFound();
                }

            ViewBag.AdminSelectList = new SelectList(_context.tblAdmin, "AdminID", "Email", institute.AdminID);
            return View(institute);
            }

        // POST: Institute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
            {
            var institute = await _context.tblInstitute.FindAsync(id);
            _context.tblInstitute.Remove(institute);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Institute removed successfully!";
            return RedirectToAction(nameof(Index));
            }

        private bool InstituteExists(int id)
            {
            return _context.tblInstitute.Any(e => e.InstituteID == id);
            }
        }
    }
