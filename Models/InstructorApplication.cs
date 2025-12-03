// Models/InstructorApplication.cs
using System.ComponentModel.DataAnnotations;
using System;

namespace WebProgramlamaProje.Models
{
    public class InstructorApplication
    {
        public int Id { get; set; }

        public string ApplicantId { get; set; } = string.Empty;
        // Başvuran kullanıcının Navigation Property'si
        public ApplicationUser Applicant { get; set; } = null!;

        [StringLength(500)]
        public string ExperienceSummary { get; set; } = string.Empty;

        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false; // Admin onayladı mı?
    }
}