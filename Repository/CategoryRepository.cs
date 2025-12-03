// Repository/CategoryRepository.cs

using System.Collections.Generic;
using System.Linq;
using WebProgramlamaProje.Models;
using WebProgramlamaProje.Data;
// DbContext'in bulunduğu Data klasörünü işaret etmeli!

namespace WebProgramlamaProje.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor Injection: DbContext'i enjekte ediyoruz.
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ICategoryRepository Metotlarının Uygulanması (Tüm Hatalı Metotlar Burada Tanımlanıyor) ---

        // Hata Çözümü: 'GetAll' tanımı içeriyor
        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        // Hata Çözümü: 'GetById' tanımı içeriyor
        public Category GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        // Hata Çözümü: 'Add' tanımı içeriyor
        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        // Hata Çözümü: 'Update' tanımı içeriyor
        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        // Hata Çözümü: 'Delete' tanımı içeriyor
        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }

        // Hata Çözümü: 'Save' tanımı içeriyor
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}