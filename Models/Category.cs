// Models/Category.cs
using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Category
    {
        public int CategoryId { get; set; } // Primary Key

        [Required(ErrorMessage = "Kategori Adı alanı boş bırakılamaz.")] // Ödev Zorunluluğu: Data Annotations
        [StringLength(50)]
        // Null uyarılarını gidermek için string.Empty atıyoruz.
        public string Name { get; set; } = string.Empty;

        // Navigation Property: Bir Kategoriye birden çok Kurs bağlı olabilir (One-to-Many).
        // Navigation Property'lere varsayılan List atayarak null uyarılarını gideriyoruz.
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}