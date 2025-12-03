// ViewModels/CourseDetailsViewModel.cs

using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.ViewModels
{
    public class CourseDetailsViewModel
    {
        public Course Course { get; set; }
        public bool IsEnrolled { get; set; }
        public int EnrollmentCount { get; set; }
        public List<ApplicationUser> EnrolledStudents { get; set; } = new();

    }
}