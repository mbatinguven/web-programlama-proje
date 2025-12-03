// ViewModels/CourseCreateViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebProgramlamaProje.Models;
using Microsoft.AspNetCore.Http; // IFormFile için

namespace WebProgramlamaProje.ViewModels
{
    public class CourseCreateViewModel
    {
        public int CourseId { get; set; } // Düzenleme için kritik

        [Required(ErrorMessage = "Kurs Başlığı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Kurs Başlığı")]
        public string Title { get; set; } = string.Empty; // Nullable çözümü

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty; // Nullable çözümü

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        // 🚨 EK EKLENDİ (Görsel ve Düzenleme Alanları)
        [Display(Name = "Mevcut Görsel Yolu")]
        public string ExistingImageUrl { get; set; } = string.Empty;

        [Display(Name = "Yeni Görsel Yükle (Değiştirmek için)")]
        public IFormFile? CourseImage { get; set; } // 👈 Hata Kaynağı Giderildi: Create/Edit formunda kullanılır

        public List<Category> Categories { get; set; } = new List<Category>();

        [Required]
        [Display(Name = "Süre")]
        [StringLength(50)]
        public string Duration { get; set; } = "Belirtilmedi";

        [Required]
        [Display(Name = "Seviye")]
        [StringLength(50)]
        public string Level { get; set; } = "Orta Seviye";

        [Required]
        [Display(Name = "Dil")]
        [StringLength(50)]
        public string Language { get; set; } = "Türkçe";

        [Required]
        [Display(Name = "Fiyat")]
        [Range(0, 10000, ErrorMessage = "Fiyat 0-10000 arası olmalıdır.")]
        public decimal Price { get; set; } = 0m;
    }
}