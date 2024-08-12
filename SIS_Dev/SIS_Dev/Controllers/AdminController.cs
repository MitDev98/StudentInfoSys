using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class AdminController : Controller
        {
        private readonly SchoolContext _context;

        public AdminController(SchoolContext context)
            {
            _context = context;
            }

        // GET: Admin
        public async Task<IActionResult> Index()
            {
            var institutes = await _context.tblInstitute.ToDictionaryAsync(c => c.InstituteID, c => c.Name);
            ViewBag.Institutes = institutes;

            return View(_context.tblAdmin.ToList());
            }



        // GET: Admin/Create
        public IActionResult Create()
            {
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name");
            return View();
            }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Admin admin)
            {
            if (ModelState.IsValid)
                {
                _context.tblAdmin.Add(admin);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Admin Added successfully!";

                return RedirectToAction(nameof(Index));
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", admin.InstituteID);
            return View(admin);
            }

        // GET: Admin/Edit/5
        public IActionResult Edit(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var admin = _context.tblAdmin.Find(id);
            if (admin == null)
                {
                return NotFound();
                }
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", admin.InstituteID);
            return View(admin);
            }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Admin admin)
            {
            if (id != admin.AdminID)
                {
                return NotFound();
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    _context.Update(admin);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Admin Updated successfully!";

                    }
                catch (DbUpdateConcurrencyException)
                    {
                    if (!AdminExists(admin.AdminID))
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
            ViewBag.InstituteID = new SelectList(_context.tblInstitute, "InstituteID", "Name", admin.InstituteID);
            return View(admin);
            }

        // GET: Admin/Delete/5
        public IActionResult Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }

            var admin = _context.tblAdmin
                .FirstOrDefault(m => m.AdminID == id);
            if (admin == null)
                {
                return NotFound();
                }

            return View(admin);
            }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
            {
            var admin = _context.tblAdmin.Find(id);
            _context.tblAdmin.Remove(admin);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Admin Deleted successfully!";

            return RedirectToAction(nameof(Index));
            }

        private bool AdminExists(int id)
            {
            return _context.tblAdmin.Any(e => e.AdminID == id);
            }
        }
    }
