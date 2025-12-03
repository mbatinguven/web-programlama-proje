// Controllers/CategoryController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebProgramlamaProje.Models;
using WebProgramlamaProje.Repository;
using WebProgramlamaProje.ViewModels; // View Models zorunluluğu için eklendi

namespace WebProgramlamaProje.Controllers
{
    public class CategoryController : Controller
    {
        // Alan (Field): ICategoryRepository arabirimini tutar.
        private readonly ICategoryRepository _categoryRepository;

        // 💡 Constructor Injection (DI) - ZORUNLU
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // --- Kategori CRUD İşlemleri ---

        // 🔐 1. Kategorileri Listeleme (Read)
        // Ödev Zorunluluğu: Rol Bazlı Yetkilendirme (Sadece Admin için)
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            // 1. Veri Repository'den çekilir. (Repository Pattern)
            var categories = _categoryRepository.GetAll();

            // 2. Entity'ler View Model'e dönüştürülür. (View Models Zorunluluğu)
            var viewModel = new CategoryListViewModel
            {
                Categories = categories
            };

            // 3. View'a View Model gönderilir.
            return View(viewModel);
        }

        // 🔐 2. Kategori Ekleme (Create - GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Create formu Category Entity'sini doğrudan kullanabiliriz, çünkü basit bir form.
            return View();
        }

        // 🔐 2. Kategori Ekleme (Create - POST)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Category category)
        {
            // Ödev Zorunluluğu: Veri Doğrulama (Data Validation)
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(category);
                _categoryRepository.Save();
                return RedirectToAction(nameof(Index));
            }

            // Doğrulama başarısızsa, hatalı veriyi içeren formu tekrar göster.
            return View(category);
        }

        // 🔐 3. Kategori Güncelleme (Update - GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            // Edit formu da basit olduğu için Entity modelini kullanabiliriz.
            return View(category);
        }

        // 🔐 3. Kategori Güncelleme (Update - POST)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category);
                _categoryRepository.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 🔐 4. Kategori Silme (Delete - POST)
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category != null)
            {
                _categoryRepository.Delete(category);
                _categoryRepository.Save();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}