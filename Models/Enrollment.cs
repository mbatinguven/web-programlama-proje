// Models/Enrollment.cs
using System;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; } // Primary Key

        // Foreign Key: Kurs
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!; // Navigation Property

        // Foreign Key: Öğrenci
        public string StudentId { get; set; } = string.Empty;
        // ApplicationUser Student { get; set; } // ApplicationUser eklendiğinde açılacak
        public ApplicationUser Student { get; set; } = null!;

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    }
}