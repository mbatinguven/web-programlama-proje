// Repository/ICategoryRepository.cs

using System.Collections.Generic;
using WebProgramlamaProje.Models; // <-- Proje adı kullanıldı

// Namespace'in doğru olduğundan emin ol.
namespace WebProgramlamaProje.Repository
{
    public interface ICategoryRepository
    {
        // CRUD: Read (Oku)
        IEnumerable<Category> GetAll();
        Category GetById(int id);

        // CRUD: Create, Update, Delete (Oluştur, Güncelle, Sil)
        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);

        // Kaydetme işlemi
        void Save();
    }
}