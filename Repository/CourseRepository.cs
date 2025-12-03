// Repository/CourseRepository.cs
using System.Collections.Generic;
using System.Linq;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebProgramlamaProje.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Kurs CRUD ve Okuma Metotları ---

        public IEnumerable<Course> GetAllCourses()
        {
            // Category ve Instructor dahil edildi.
            return _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .ToList();
        }

        public Course? GetCourseById(int id)
        {
            // Category, Instructor ve Modüller dahil edildi.
            return _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Modules)
                .FirstOrDefault(c => c.CourseId == id);
        }
        public void DeleteModule(Module module)
        {
            _context.Modules.Remove(module);
        }
        public void AddCourse(Course course) => _context.Courses.Add(course);
        public void UpdateCourse(Course course) => _context.Courses.Update(course);
        public void DeleteCourse(Course course) => _context.Courses.Remove(course);

        // --- Öğrenci Kayıt ve Sorgulama Metotları ---

        public IEnumerable<Enrollment> GetEnrollmentsByCourse(int courseId)
        {
            // Öğrenci bilgisi dahil edildi.
            return _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .ToList();
        }

        public bool IsStudentEnrolled(int courseId, string studentId)
        {
            return _context.Enrollments
                .Any(e => e.CourseId == courseId && e.StudentId == studentId);
        }

        public void EnrollStudent(int courseId, string studentId)
        {
            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                EnrollmentDate = DateTime.Now
            };
            _context.Enrollments.Add(enrollment);
        }

        public IEnumerable<Enrollment> GetCoursesByStudent(string? studentId)
        {
            // Null kontrolü yapıldı
            if (studentId == null) return Enumerable.Empty<Enrollment>();

            // Kurs, Kategori ve Enrollment bilgileri dahil edildi.
            return _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Category)
                .ToList();
        }

        // --- Modül CRUD Metotları ---

        public Module? GetModuleById(int id)
        {
            return _context.Modules.FirstOrDefault(m => m.ModuleId == id);
        }

        public void AddModule(Module module) => _context.Modules.Add(module);

        public void UpdateModule(Module module)
        {
            _context.Modules.Update(module);
        }

        // --- Eğitmen Başvuru Yönetimi (Yeni Eklendi) ---

        public void AddInstructorApplication(InstructorApplication application)
        {
            _context.InstructorApplications.Add(application);
        }

        public bool HasPendingApplication(string applicantId)
        {
            return _context.InstructorApplications
                .Any(a => a.ApplicantId == applicantId && !a.IsApproved);
        }

        public List<InstructorApplication> GetPendingApplications()
        {
            // Admin panelinde listelemek için başvuran kullanıcıyı (Applicant) dahil ediyoruz.
            return _context.InstructorApplications
                .Include(a => a.Applicant)
                .Where(a => !a.IsApproved) // Onaylanmamış olanları getir
                .ToList();
        }

        public void MarkApplicationApproved(int applicationId)
        {
            var application = _context.InstructorApplications.Find(applicationId);
            if (application != null)
            {
                application.IsApproved = true;
                _context.InstructorApplications.Update(application);
            }
        }

        

        public IEnumerable<Course> GetCoursesByInstructor(string? instructorId)
        {
            if (string.IsNullOrEmpty(instructorId)) return Enumerable.Empty<Course>();

            // Sadece bu EğitmenID'sine ait kursları çek
            return _context.Courses
                .Include(c => c.Category)
                .Where(c => c.InstructorId == instructorId)
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}