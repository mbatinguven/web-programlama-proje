// Repository/ICourseRepository.cs
using System.Collections.Generic;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Repository
{
    public interface ICourseRepository
    {
        // --- 1. Kurs CRUD İşlemleri ---
        IEnumerable<Course> GetAllCourses();
        Course? GetCourseById(int id);
        void AddCourse(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);

        // --- 2. Öğrenci Kayıt (Enrollment) İşlemleri ---
        void EnrollStudent(int courseId, string studentId);
        bool IsStudentEnrolled(int courseId, string studentId);
        IEnumerable<Enrollment> GetEnrollmentsByCourse(int courseId);
        IEnumerable<Enrollment> GetCoursesByStudent(string? studentI);

        IEnumerable<Course> GetCoursesByInstructor(string? instructorId);

        // --- 3. Modül CRUD İşlemleri ---
        Module? GetModuleById(int id);
        void AddModule(Module module);
        void UpdateModule(Module module);
        // DeleteModule metodu da eklenebilir.

        // --- 4. Eğitmen Başvuru Yönetimi (Rol Onayı Mantığı) ---
        void AddInstructorApplication(InstructorApplication application);
        bool HasPendingApplication(string applicantId);
        List<InstructorApplication> GetPendingApplications();
        void MarkApplicationApproved(int applicationId);

        // --- 5. Kayıt ---
        void Save();
        void DeleteModule(Module module);
    }
}