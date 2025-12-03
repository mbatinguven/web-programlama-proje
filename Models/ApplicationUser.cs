// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using WebProgramlamaProje.Models;
using System.Collections.Generic;

namespace WebProgramlamaProje.Models
{
    // IdentityUser'dan miras alır
    public class ApplicationUser : IdentityUser
    {
        // Ek özellikler buraya eklenebilir (Örn: FirstName, LastName)

        // Navigation Property: Eğitmen bu kursları açar (One-to-Many)
        public ICollection<Course> CoursesTaught { get; set; } = new List<Course>();

        // Navigation Property: Öğrenci bu kayıtlara sahiptir
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}