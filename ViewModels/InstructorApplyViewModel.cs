// ViewModels/InstructorApplyViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.ViewModels
{
    public class InstructorApplyViewModel
    {
        [Required(ErrorMessage = "Lütfen deneyiminizi ve tecrübenizi özetleyiniz.")]
        [StringLength(500, ErrorMessage = "Özet en fazla 500 karakter olabilir.")]
        [Display(Name = "Deneyim Özeti ve Motivasyon")]
        public string ExperienceSummary { get; set; } = string.Empty;

        // İleride dosya yükleme (CV) eklenmek istenirse burası genişletilebilir.
        // public IFormFile? CVFile { get; set; } 
    }
}