// Models/Module.cs

using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Module
    {
        public int ModuleId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty; // Nullable çözüldü

        public string Content { get; set; } = string.Empty; // Nullable çözüldü

        // Hatanın geldiği Description alanı Modül'de yoktu, Course'da vardı.
        // Eğer View Model'de kullanılıyorsa eklenmeli. Şimdilik bu haliyle devam ediyoruz.

        // Foreign Key: Hangi Kursa ait
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!; // Course modeline bağlı

        [StringLength(50)]
        [Display(Name = "YouTube Video ID")]
        public string VideoId { get; set; } = string.Empty;

    }
}