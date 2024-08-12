using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;

namespace SIS_Dev.Controllers
    {
    public class AccountController : Controller
        {
        private readonly SchoolContext _context;

        public AccountController(SchoolContext context)
            {
            _context = context;
            }

        [HttpGet]
        public IActionResult Login()
            {
            return View();
            }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
            {

            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                ModelState.AddModelError("", "Email and password are required.");
                return View(user);
                }

            try
                {
                // Check superadmin credentials
                var superadmin = await _context.tblSuperadmin
                    .FirstOrDefaultAsync(a => a.Email == user.Email && a.Password == user.Password);
                if (superadmin != null)
                    {
                    HttpContext.Session.SetString("UserEmail", superadmin.Email);
                    HttpContext.Session.SetString("UserRole", "SuperAdmin");
                    return RedirectToAction("Index", "Home");
                    }

                //// Check admin credentials
                //var admin = await _context.tblAdmin
                //    .FirstOrDefaultAsync(a => a.Email == user.Email && a.Password == user.Password);
                //if (admin != null)
                //    {
                //    HttpContext.Session.SetString("UserEmail", admin.Email);
                //    HttpContext.Session.SetString("UserRole", "Admin");
                //    return RedirectToAction("Index", "Home");
                //    }

                // Check admin credentials
                var admin = await _context.tblAdmin
                    .FirstOrDefaultAsync(a => a.Email == user.Email && a.Password == user.Password);
                if (admin != null)
                    {
                    HttpContext.Session.SetString("UserEmail", admin.Email);
                    HttpContext.Session.SetString("UserRole", "Admin");
                    HttpContext.Session.SetInt32("InstituteID", admin.InstituteID); // Store InstituteID in Session
                    return RedirectToAction("Index", "Home");
                    }

                // Check faculty credentials
                var faculty = await _context.tblFaculties
                    .FirstOrDefaultAsync(f => f.Email == user.Email && f.Password == user.Password);
                if (faculty != null)
                    {
                    HttpContext.Session.SetString("UserEmail", faculty.Email);
                    HttpContext.Session.SetString("UserRole", "Faculty");
                    return RedirectToAction("Index", "Home");
                    }

                // Check student credentials
                var student = await _context.tblStudent
                    .FirstOrDefaultAsync(s => s.Email == user.Email && s.Password == user.Password);
                if (student != null)
                    {
                    HttpContext.Session.SetString("UserEmail", student.Email);
                    HttpContext.Session.SetString("UserRole", "Student");
                    HttpContext.Session.SetInt32("StudentID", student.StudentID);
                    return RedirectToAction("Index", "Home");
                    }

                // Invalid login attempt
                ModelState.AddModelError("", "Invalid email or password");
                return View(user);
                }
            catch (Exception ex)
                {
                // Log the exception
                //_logger.LogError($"An error occurred: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View(user);
                }
            }

        public IActionResult Logout()
            {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
            }
        }
    }









