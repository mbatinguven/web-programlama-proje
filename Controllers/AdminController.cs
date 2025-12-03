using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebProgramlamaProje.Models;
using WebProgramlamaProje.Repository;
using WebProgramlamaProje.ViewModels;

namespace WebProgramlamaProje.Controllers
{
    // Controller seviyesinde tüm aksiyonlar Admin için kilitli
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // Roller için eklendi

        // DI (Dependency Injection)
        public AdminController(ICourseRepository courseRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _courseRepository = courseRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // --- 1. ADMIN DASHBOARD (ANASAYFA) ---
        public IActionResult Index()
        {
            return View();
        }

        // --- 2. KULLANICI YÖNETİMİ ---
        public async Task<IActionResult> ManageUsers()
        {
            // Veritabanından tüm rol listelerini çekiyoruz.
            var instructors = (await _userManager.GetUsersInRoleAsync("Instructor")).ToList();
            var students = (await _userManager.GetUsersInRoleAsync("Student")).ToList();
            var admins = (await _userManager.GetUsersInRoleAsync("Admin")).ToList();

            // 💡 LİSTE TEMİZLEME MANTIĞI (Çakışmaları Giderme)
            // Eğer bir kullanıcı hem Admin hem Instructor ise, Öğrenci/Eğitmen listesinde gösterilmemeli.
            // Eğer bir kullanıcı Instructor ise, Öğrenci listesinde gösterilmemeli.
            var allAdmins = admins.Select(u => u.Id).ToHashSet();
            var allInstructors = instructors.Select(u => u.Id).ToHashSet();

            // SADECE ÖĞRENCİ olanları bul (Ne Admin ne de Instructor olanlar)
            var pureStudents = students
                .Where(s => !allAdmins.Contains(s.Id) && !allInstructors.Contains(s.Id))
                .ToList();

            // SADECE EĞİTMEN olanları bul (Admin olmayanlar)
            var pureInstructors = instructors
                .Where(i => !allAdmins.Contains(i.Id))
                .ToList();

            var viewModel = new UserManagementViewModel
            {
                Instructors = pureInstructors, // Sadece Öğretmenler
                Students = pureStudents,     // Sadece Öğrenciler
                Admins = admins              // Tüm Adminler
            };

            return View(viewModel); // Views/Admin/ManageUsers.cshtml
        }

        // --- 3. KULLANICI ROLÜNÜ DEĞİŞTİRME/BLOKLAMA ---
        [HttpPost]
        public async Task<IActionResult> ToggleInstructorRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Ödev zorunluluğu: Admin kendi rolünü bu ekrandan değiştirememelidir.
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Yönetici rolü bu ekran üzerinden değiştirilemez.";
                return RedirectToAction(nameof(ManageUsers));
            }

            if (await _userManager.IsInRoleAsync(user, "Instructor"))
            {
                // Rolü sil (Öğretmenlikten çıkar)
                await _userManager.RemoveFromRoleAsync(user, "Instructor");
                TempData["SuccessMessage"] = $"{user.Email} kullanıcısının Eğitmenlik rolü kaldırıldı.";
            }
            else
            {
                // Rolü ekle (Öğretmen yap)
                await _userManager.AddToRoleAsync(user, "Instructor");
                TempData["SuccessMessage"] = $"{user.Email} kullanıcısına Eğitmen rolü atandı.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        // KULLANICI SİLME (PERMANENT DELETE)
        // --------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // 1. Güvenlik Kontrolü: Admin, kendi hesabını silememeli.
            if (user.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                TempData["ErrorMessage"] = "Kendi hesabınızı silemezsiniz.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // 2. Silme İşlemini Gerçekleştirme
            // EF Core'daki Cascade Delete ayarlarıyla kullanıcının tüm verileri (Enrollment, vb.) otomatik silinir.
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"{user.Email} kullanıcısı sistemden kalıcı olarak silindi.";
            }
            else
            {
                // Silme işlemi başarısız olursa (örneğin hala bir kursa bağlıysa ve Cascade Delete çalışmazsa)
                TempData["ErrorMessage"] = $"Kullanıcı silinirken hata oluştu: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(ManageUsers));
        }
    }
}