// ViewModels/ModuleCreateViewModel.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebProgramlamaProje.Models;
using Microsoft.AspNetCore.Http;

namespace WebProgramlamaProje.ViewModels
{
    public class ModuleCreateViewModel
    {
        // CRUD İÇİN ID'ler
        public int ModuleId { get; set; }
        public int CourseId { get; set; }

        // GÖRÜNÜM VERİSİ
        public string CourseTitle { get; set; } = string.Empty;

        // EĞİTMEN KURS RESMİ (Gerekirse)
        public string ExistingImageUrl { get; set; } = string.Empty;

        // FORM ALANLARI
        [Required(ErrorMessage = "Modül başlığı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Modül Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur.")]
        [Display(Name = "Modül İçeriği")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "YouTube Video ID")]
        [StringLength(255)]
        public string VideoLink { get; set; } = string.Empty;

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Module> ExistingModules { get; set; } = new List<Module>();

    }
}