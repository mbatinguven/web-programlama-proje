// Models/Course.cs

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Models
{
    public class Course
    {
        public int CourseId { get; set; } // Primary Key

        [Required(ErrorMessage = "Kurs Başlığı zorunludur.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty; // Nullable sorunu çözüldü

        public string Description { get; set; } = string.Empty; // Nullable sorunu çözüldü

        // --- Yeni Eklenen Özellikler ---
        // 💡 Hata Çözümü: 'MaxEnrollment' tanımı içermiyor
        public int MaxEnrollment { get; set; }

        // 💡 Hata Çözümü: 'IsActive' tanımı içermiyor
        public bool IsActive { get; set; } = true;
        // ---------------------------------

        // Foreign Key: Kategori
        public int CategoryId { get; set; }
        // 💡 Hata Çözümü: Navigation Property'leri EF Core'un dolduracağını belirtiyoruz.
        public Category Category { get; set; } = null!;

        // Foreign Key: Eğitmen
        public string InstructorId { get; set; } = string.Empty; // Nullable sorunu çözüldü
        public ApplicationUser Instructor { get; set; } = null!;

        // Navigation Property: Kursa kayıtlı öğrenciler
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // Navigation Property: Kurs modülleri
        public ICollection<Module> Modules { get; set; } = new List<Module>();
        
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty; // Yüklenen resmin yolu

        [StringLength(50)]
        public string Duration { get; set; } = "Belirtilmedi";  // Ör: "12 saat", "5 hafta"

        [StringLength(50)]
        public string Level { get; set; } = "Orta Seviye";      // Ör: "Başlangıç", "Orta", "İleri"

        [StringLength(50)]
        public string Language { get; set; } = "Türkçe";        // Ör: "Türkçe", "İngilizce"

        public decimal Price { get; set; } = 0m;
    }
}