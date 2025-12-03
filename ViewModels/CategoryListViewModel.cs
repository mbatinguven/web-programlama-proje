// ViewModels/CategoryListViewModel.cs

using WebProgramlamaProje.Models;
using System.Collections.Generic;

namespace WebProgramlamaProje.ViewModels
{
    // Index View'ının (Kategori Listesi) ihtiyacı olan tüm veriyi tutar.
    public class CategoryListViewModel
    {
        // View'ın asıl listeye ihtiyacı var.
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        // İstenirse sayfalama (Pagination) bilgisi de buraya eklenebilir.
        // public int CurrentPage { get; set; }
        // public int TotalPages { get; set; }
    }
}