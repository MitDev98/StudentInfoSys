
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using SIS_Dev.Models;

//namespace SIS_Dev.Controllers
//    {
//    public class AttendanceController : Controller
//        {
//        private readonly SchoolContext _context;

//        public AttendanceController(SchoolContext context)
//            {
//            _context = context;
//            }

//        public IActionResult Index()
//            {

//            ViewBag.Courses = _context.tblCourse.ToList();
//            ViewBag.Standards = _context.tblStudent.Select(s => s.Standard).Distinct().ToList();

//            return View();
//            }

//        [HttpPost]
//        public async Task<IActionResult> GetStudents(int courseId, string standard)
//            {
//            var students = await _context.tblStudent
//                .Where(s => s.CourseID == courseId && s.Standard == standard)
//                .ToListAsync();
//            return Json(students);
//            }

//        [HttpPost]
//        public async Task<IActionResult> GetSubjects(int courseId)
//            {
//            var subjects = await _context.tblSubject
//                .Where(s => s.CourseID == courseId)
//                .Select(s => new { s.SubjectID, s.SubjectName }) // Select only needed properties
//                .ToListAsync();
//            return Json(subjects);
//            }
//        [HttpPost]
//        public async Task<IActionResult> SubmitAttendance(List<AttendanceViewModel> attendanceList)
//            {
//            if (!ModelState.IsValid)
//                {
//                return BadRequest(ModelState);
//                }
//            try
//                {
//                var submittedAttendance = new List<Attendance>();

//                foreach (var attendance in attendanceList)
//                    {
//                    var newAttendance = new Attendance
//                        {
//                        Date = DateTime.Now,
//                        StudentID = attendance.StudentId,
//                        Status = attendance.IsPresent,
//                        SubjectName = attendance.SubjectName,
//                        StartTime = attendance.StartTime,
//                        EndTime = attendance.EndTime,
//                        Topic = attendance.Topic
//                        };
//                    _context.tblAttendance.Add(newAttendance);
//                    submittedAttendance.Add(newAttendance);
//                    }
//                await _context.SaveChangesAsync();

//                return RedirectToAction("Confirmation", new { attendanceData = JsonConvert.SerializeObject(submittedAttendance) });
//                }
//            catch (DbUpdateException ex)
//                {
//                return StatusCode(500, $"Failed to submit attendance: {ex.Message}");
//                }
//            }


//        public IActionResult Confirmation(string attendanceData)
//            {
//            var attendanceList = JsonConvert.DeserializeObject<List<Attendance>>(attendanceData);
//            return View(attendanceList);
//            }

//        }
//    }
