using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIS_Dev.Models;
using System.Diagnostics;

namespace SIS_Dev.Controllers
    {
    public class HomeController : Controller
        {
        private readonly SchoolContext _context;

        public HomeController(SchoolContext context)
            {
            _context = context;
            }


        public IActionResult Index()
            {
            // Retrieve role from session
            var userRole = HttpContext.Session.GetString("UserRole");


            // Redirect based on user role
            switch (userRole)
                {
                case "SuperAdmin":
                    return RedirectToAction("SuperAdminDashboard");
                case "Admin":
                    return RedirectToAction("AdminDashboard");
                case "Faculty":
                    return RedirectToAction("FacultyHome");
                case "Student":
                    return RedirectToAction("StudentHome");
                default:
                    return RedirectToAction("Login", "Account");
                }
            }

        public async Task<IActionResult> SuperAdminDashboard()
            {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userEmail == null)
                {
                return RedirectToAction("Login", "Account");
                }

            ViewBag.UserName = userEmail;
            ViewBag.UserRole = "SuperAdmin";

            int totalStudents = await _context.tblStudent.CountAsync();
            int totalFaculties = await _context.tblFaculties.CountAsync();
            int totalCourses = await _context.tblCourse.CountAsync();
            int totalSubjects = await _context.tblSubject.CountAsync();
            int totalAssessmentResults = await _context.tblAssessmentResult.CountAsync();
            int totalInstitutes = await _context.tblInstitute.CountAsync();



            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalFaculties = totalFaculties;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalSubjects = totalSubjects;
            ViewBag.TotalAssessmentResults = totalAssessmentResults;
            ViewBag.TotalInstitutes = totalInstitutes;


            return View();
            }


        public async Task<IActionResult> AdminDashboard()
            {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userEmail == null)
                {
                return RedirectToAction("Login", "Account");
                }

            var admin = await _context.tblAdmin.SingleOrDefaultAsync(a => a.Email == userEmail);
            if (admin == null)
                {
                return RedirectToAction("Login", "Account");
                }

            ViewBag.UserName = userEmail;
            ViewBag.UserRole = "Admin";

            int totalStudents = await _context.tblStudent.CountAsync(s => s.InstituteID == admin.InstituteID);
            int totalFaculties = await _context.tblFaculties.CountAsync(f => f.InstituteID == admin.InstituteID);
            int totalCourses = await _context.tblCourse.CountAsync(c => c.InstituteID == admin.InstituteID);
            int totalSubjects = await _context.tblSubject.CountAsync(s => s.InstituteID == admin.InstituteID);

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalFaculties = totalFaculties;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalSubjects = totalSubjects;

            return View();
            }
        public async Task<IActionResult> AdminDashboard2()
            {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userEmail == null)
                {
                return RedirectToAction("Login", "Account");
                }

            var admin = await _context.tblAdmin.SingleOrDefaultAsync(a => a.Email == userEmail);
            if (admin == null)
                {
                return RedirectToAction("Login", "Account");
                }

            ViewBag.UserName = userEmail;
            ViewBag.UserRole = "Admin1";

            int totalStudents = await _context.tblStudent.CountAsync(s => s.InstituteID == admin.InstituteID);
            int totalFaculties = await _context.tblFaculties.CountAsync(f => f.InstituteID == admin.InstituteID);
            int totalCourses = await _context.tblCourse.CountAsync(c => c.InstituteID == admin.InstituteID);
            int totalSubjects = await _context.tblSubject.CountAsync(s => s.InstituteID == admin.InstituteID);

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalFaculties = totalFaculties;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalSubjects = totalSubjects;

            return View();
            }
        public async Task<IActionResult> AdminDashboard3()
            {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userEmail == null)
                {
                return RedirectToAction("Login", "Account");
                }

            var admin = await _context.tblAdmin.SingleOrDefaultAsync(a => a.Email == userEmail);
            if (admin == null)
                {
                return RedirectToAction("Login", "Account");
                }

            ViewBag.UserName = userEmail;
            ViewBag.UserRole = "Admin1";

            int totalStudents = await _context.tblStudent.CountAsync(s => s.InstituteID == admin.InstituteID);
            int totalFaculties = await _context.tblFaculties.CountAsync(f => f.InstituteID == admin.InstituteID);
            int totalCourses = await _context.tblCourse.CountAsync(c => c.InstituteID == admin.InstituteID);
            int totalSubjects = await _context.tblSubject.CountAsync(s => s.InstituteID == admin.InstituteID);

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalFaculties = totalFaculties;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalSubjects = totalSubjects;

            return View();
            }

        public async Task<IActionResult> FacultyHome()
            {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userEmail == null)
                {
                return RedirectToAction("Login", "Account");
                }
            var student = await _context.tblFaculties.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null)
                {
                // Handle case where student is not found (optional)
                return RedirectToAction("Login", "Account");
                }

            ViewBag.UserName = userEmail;
            ViewBag.UserRole = "Faculty";

            int totalStudents = await _context.tblStudent.CountAsync();
            ViewBag.TotalStudents = totalStudents;

            int totalAssessments = await _context.tblAssessment
          .CountAsync(ar => ar.FacultyID == student.FacultyID); // Assuming StudentId is the foreign key in tblAssessmentResult

            ViewBag.TotalAssessments = totalAssessments;

            // Add any data retrieval and logic specific to the faculty dashboard here

            return View();
            }

        public async Task<IActionResult> StudentHome()
            {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userEmail == null)
                {
                return RedirectToAction("Login", "Account");
                }
            var student = await _context.tblStudent.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null)
                {
                // Handle case where student is not found (optional)
                return RedirectToAction("Login", "Account");
                }

            ViewBag.UserName = userEmail;
            ViewBag.UserRole = "Student";

            int totalAssessmentResults = await _context.tblAssessmentResult
          .CountAsync(ar => ar.StudentID == student.StudentID); // Assuming StudentId is the foreign key in tblAssessmentResult

            ViewBag.TotalAssessmentResults = totalAssessmentResults;
            return View();
            }


        public IActionResult Privacy()
            {
            return View();
            }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
