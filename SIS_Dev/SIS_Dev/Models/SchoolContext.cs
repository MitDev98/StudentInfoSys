using Microsoft.EntityFrameworkCore;

namespace SIS_Dev.Models
    {
    public class SchoolContext : DbContext
        {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<Student> tblStudent { get; set; }
        public DbSet<Faculty> tblFaculties { get; set; }
        public DbSet<Role> tblRole { get; set; }
        public DbSet<User> tblUser { get; set; }
        public DbSet<UserDetails> tblUserdetails { get; set; }
        public DbSet<Course> tblCourse { get; set; }
        public DbSet<Department> tblDepartment { get; set; }
        public DbSet<Subject> tblSubject { get; set; }
        public DbSet<Timetable> tblTimetable { get; set; }
        public DbSet<TimetableDetails> tblTimetableDetails { get; set; }
        public DbSet<Counsellor> tblCounsellor { get; set; }
        public DbSet<Assessment> tblAssessment { get; set; }
        public DbSet<Attendance> tblAttendence { get; set; }
        public DbSet<AssessmentResult> tblAssessmentResult { get; set; }
        public DbSet<Institute> tblInstitute { get; set; }
        public DbSet<Standard> tblStandard { get; set; }
        public DbSet<Leave> tblLeaves { get; set; }
        public DbSet<Superadmin> tblSuperadmin { get; set; }
        public DbSet<Admin> tblAdmin { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {


            }


        }
    }
