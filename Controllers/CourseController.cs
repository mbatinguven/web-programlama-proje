using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebProgramlamaProje.Models;
using WebProgramlamaProje.Repository;
using WebProgramlamaProje.ViewModels;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO; // Dosya işlemleri için
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment için

namespace WebProgramlamaProje.Controllers
{
    [Authorize] // Controller seviyesinde tüm aksiyonlara giriş zorunluluğu eklenir.
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CourseController(
            ICourseRepository courseRepository,
              ICategoryRepository categoryRepository,
              UserManager<ApplicationUser> userManager,
               IWebHostEnvironment hostEnvironment)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // --- TEMEL AKIŞLAR (Index, Details, MyCourses, Create, Edit, Delete) ---

        public IActionResult Index(int? categoryId) // Yeni parametre eklendi
        {
            // Kategori listesini View'a taşı
            ViewData["Categories"] = _categoryRepository.GetAll().ToList();
            ViewData["CurrentCategoryId"] = categoryId; // Seçili ID'yi View'a gönder

            IEnumerable<Course> courses;

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                // FİLTRELEME MANTIĞI: Sadece seçilen kategoriye ait kursları çek
                courses = _courseRepository.GetAllCourses().Where(c => c.CategoryId == categoryId.Value);
            }
            else
            {
                // Filtre yoksa tüm kursları çek
                courses = _courseRepository.GetAllCourses();
            }

            var viewModel = new CourseListViewModel { Courses = courses };
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var course = _courseRepository.GetCourseById(id);
            if (course == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isInstructorOwner = false;
            if (!string.IsNullOrEmpty(userId))
            {
                isInstructorOwner = course.InstructorId == userId;
            }

            var enrolledStudents = _courseRepository
                .GetEnrollmentsByCourse(id)
                .Select(e => e.Student)
                .ToList();

            var viewModel = new CourseDetailsViewModel
            {
                Course = course,
                IsEnrolled = (userId != null)
                    ? _courseRepository.IsStudentEnrolled(id, userId)
                    : false,
                EnrollmentCount = enrolledStudents.Count,
                EnrolledStudents = enrolledStudents
            };

            ViewBag.IsInstructorOwner = isInstructorOwner;

            return View(viewModel);
        }

        [Authorize(Roles = "Student")]
        public IActionResult MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrollments = _courseRepository.GetCoursesByStudent(userId);
            return View(enrollments.Select(e => e.Course).ToList());
        }

        [Authorize(Roles = "Instructor")]
        public IActionResult Create()
        {
            var viewModel = new CourseCreateViewModel
            {
                Categories = _categoryRepository.GetAll().ToList()
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            // ✅ 1) ÖNCE MODELSTATE KONTROLÜ ŞART
            if (!ModelState.IsValid)
            {
                model.Categories = _categoryRepository.GetAll().ToList();
                return View(model);
            }

            string imageUrl = string.Empty;

            // ✅ 2) DOSYA YÜKLEME
            if (model.CourseImage != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "courses");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CourseImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CourseImage.CopyToAsync(fileStream);
                }

                imageUrl = $"/images/courses/{uniqueFileName}";
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Oturum bilgileri eksik.";
                return RedirectToAction(nameof(Index));
            }

            // ✅ 3) COURSE OLUŞTURMA – SENİN YENİ ALANLARIN DOĞRU BAĞLANDI
            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ImageUrl = imageUrl,
                InstructorId = userId,
                MaxEnrollment = 50,
                IsActive = true,

                Duration = model.Duration,
                Level = model.Level,
                Language = model.Language,
                Price = model.Price
            };

            _courseRepository.AddCourse(course);
            _courseRepository.Save();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Student, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (!_courseRepository.IsStudentEnrolled(courseId, userId))
            {
                _courseRepository.EnrollStudent(courseId, userId);
                _courseRepository.Save();
                TempData["Message"] = "Kursa başarıyla kaydoldunuz!";
            }
            else
            {
                TempData["Message"] = "Bu kursa zaten kaydınız bulunmaktadır.";
            }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        [Authorize(Roles = "Admin, Instructor")]
        public IActionResult Edit(int id)
        {
            var course = _courseRepository.GetCourseById(id);
            if (course == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var viewModel = new CourseCreateViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,

                // ✅ YENİ ALANLAR
                Duration = course.Duration,
                Level = course.Level,
                Language = course.Language,
                Price = course.Price,
                ExistingImageUrl = course.ImageUrl,

                Categories = _categoryRepository.GetAll().ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CourseCreateViewModel model)
        {
            if (id != model.CourseId) return NotFound();

            var courseToUpdate = _courseRepository.GetCourseById(id);
            if (courseToUpdate == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (courseToUpdate.InstructorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = _categoryRepository.GetAll().ToList();
                return View(model);
            }

            courseToUpdate.Title = model.Title;
            courseToUpdate.Description = model.Description;
            courseToUpdate.CategoryId = model.CategoryId;

            // ✅ YENİ ALANLAR GÜNCELLENİYOR
            courseToUpdate.Duration = model.Duration;
            courseToUpdate.Level = model.Level;
            courseToUpdate.Language = model.Language;
            courseToUpdate.Price = model.Price;

            _courseRepository.UpdateCourse(courseToUpdate);
            _courseRepository.Save();

            TempData["SuccessMessage"] = "Kurs başarıyla güncellendi!";
            return RedirectToAction(nameof(Details), new { id = courseToUpdate.CourseId });
        }

        [Authorize(Roles = "Admin, Instructor")]
        public IActionResult Delete(int id)
        {
            var courseToDelete = _courseRepository.GetCourseById(id);
            if (courseToDelete == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (courseToDelete.InstructorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            return View(courseToDelete);
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var courseToDelete = _courseRepository.GetCourseById(id);
            if (courseToDelete == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (courseToDelete.InstructorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _courseRepository.DeleteCourse(courseToDelete);
            _courseRepository.Save();
            TempData["SuccessMessage"] = "Kurs başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------
        // MODÜL YÖNETİMİ (YÖNETİCİ/EĞİTMEN ZORUNLULUĞU)
        // --------------------------------------------------------

        [Authorize(Roles = "Instructor")]
        public IActionResult ManageModules(int courseId)
        {
            var course = _courseRepository.GetCourseById(courseId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (course == null || course.InstructorId != userId)
            {
                return Forbid();
            }

            var viewModel = new ModuleCreateViewModel
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                ExistingModules = course.Modules.ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddModule(ModuleCreateViewModel model)
        {
            var course = _courseRepository.GetCourseById(model.CourseId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Yetki ve null kontrolü
            if (course == null || course.InstructorId != userId)
            {
                return Forbid();
            }

            // geçerli model
            if (ModelState.IsValid)
            {
                string videoId = model.VideoLink ?? string.Empty;

                var module = new Module
                {
                    CourseId = model.CourseId,
                    Title = model.Title,
                    Content = model.Content,
                    VideoId = videoId
                };

                _courseRepository.AddModule(module);
                _courseRepository.Save();

                TempData["SuccessMessage"] = "Modül başarıyla eklendi!";

                return RedirectToAction(nameof(ManageModules), new { courseId = model.CourseId });
            }

            // ❗ HATA DURUMUNDA MUTLAKA MODEL'İ TEKRAR DOLDUR !!
            model.ExistingModules = course.Modules.ToList();
            model.CourseTitle = course.Title;

            // Doğru View’a geri dön
            return View("ManageModules", model);
        }


        // --------------------------------------------------------
        // MODÜL DÜZENLEME (YENİ EKLENEN AKSİYONLAR)
        // --------------------------------------------------------

        [Authorize(Roles = "Instructor")]
        public IActionResult EditModule(int id)
        {
            var module = _courseRepository.GetModuleById(id);
            if (module == null) return NotFound();

            var course = _courseRepository.GetCourseById(module.CourseId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (course == null || course.InstructorId != userId)
            {
                return Forbid();
            }

            var viewModel = new ModuleCreateViewModel
            {
                ModuleId = module.ModuleId,
                CourseId = module.CourseId,
                Title = module.Title,
                Content = module.Content,
                CourseTitle = course.Title,
                ExistingImageUrl = course.ImageUrl, // Artık View Model'de tanımlı!
                Categories = _categoryRepository.GetAll().ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditModule(int id, ModuleCreateViewModel model)
        {
            if (id != model.ModuleId) return NotFound();

            var moduleToUpdate = _courseRepository.GetModuleById(id);
            if (moduleToUpdate == null) return NotFound();

            var course = _courseRepository.GetCourseById(moduleToUpdate.CourseId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (course == null || course.InstructorId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                moduleToUpdate.Title = model.Title;
                moduleToUpdate.Content = model.Content;

                _courseRepository.UpdateModule(moduleToUpdate);
                _courseRepository.Save();
                TempData["SuccessMessage"] = "Modül başarıyla güncellendi.";

                return RedirectToAction(nameof(ManageModules), new { courseId = model.CourseId });
            }

            // Hata durumunda View Model'i tekrar doldur
           
            model.CourseTitle = course?.Title ?? "Modül Yönetimi";
            model.ExistingModules = course?.Modules.ToList() ?? new List<Module>();
            return View("EditModule", model);
        }

        // --------------------------------------------------------
        // EĞİTMEN BAŞVURU SİSTEMİ (ONAY GEREKLİ)
        // --------------------------------------------------------

        [Authorize(Roles = "Student")] // SADECE öğrenci rolündekiler başvurabilir
        public IActionResult ApplyAsInstructor()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Başvurusu ZATEN ONAY BEKLEYEN veya Instructor olan kullanıcıları kontrol et
            // Bu kontrol, View'daki linki göstermeyen mantığı destekler.
            if (_courseRepository.HasPendingApplication(userId))
            {
                TempData["InfoMessage"] = "Zaten onay bekleyen bir başvurunuz bulunmaktadır.";
                return RedirectToAction(nameof(Index)); // Kurs listesine geri gönder
            }

            // YENİ BAŞVURU FORMU
            return View(new InstructorApplyViewModel());
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyAsInstructor(InstructorApplyViewModel model)
        {
            // 1. Veri Doğrulama (Data Validation)
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Kullanıcı zaten Instructor ise, Forbid() döndürmek gerekir, 
                // ancak menü kontrolü bunu engellediği için basitçe devam ediyoruz.

                var application = new InstructorApplication
                {
                    ApplicantId = userId,
                    ExperienceSummary = model.ExperienceSummary,
                    IsApproved = false // Yönetici onayı bekliyor
                };

                // 2. Repository aracılığıyla veritabanına kaydet
                _courseRepository.AddInstructorApplication(application);
                _courseRepository.Save();

                // 3. Başarı mesajını TempData'ya kaydet (View'da gösterilecek)
                TempData["SuccessMessage"] = "Başvurunuz alındı ve yönetici onayını beklemektedir.";

                // 4. Kullanıcıyı AYNI View'a geri döndür (Başarılı mesajını göstermek için)
                // RedirectToAction yerine View(model) kullanmak, aynı sayfada kalmayı sağlar.
                return View(model);
            }

            // 5. Hata varsa formu tekrar göster
            return View(model);
        }

        // --------------------------------------------------------
        // ADMİN ONAY VE YÖNETİM AKSİYONLARI (ManageApplications)
        // --------------------------------------------------------

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageApplications()
        {
            var applications = _courseRepository.GetPendingApplications();
            return View(applications);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ApproveInstructor(string applicantId, int applicationId)
        {
            var user = await _userManager.FindByIdAsync(applicantId);
            if (user != null && !await _userManager.IsInRoleAsync(user, "Instructor"))
            {
                await _userManager.AddToRoleAsync(user, "Instructor");
            }

            _courseRepository.MarkApplicationApproved(applicationId);
            _courseRepository.Save();

            TempData["SuccessMessage"] = $"{user?.UserName} artık Eğitmendir.";
            return RedirectToAction(nameof(ManageApplications));
        }
        [Authorize(Roles = "Instructor")]
        public IActionResult MyCoursesTeaching()
        {
            // Aktif kullanıcının ID'sini (Eğitmen ID'si) al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Repository'den sadece bu Eğitmene ait kursları çek
            var courses = _courseRepository.GetCoursesByInstructor(userId);

            var viewModel = new CourseListViewModel { Courses = courses };

            // 💡 View Engine'e MyCoursesTeaching.cshtml dosyasını döndürmesini söyleriz.
            return View(viewModel);
        }
        // MODÜL SİLME (DELETE MODULE)
        // --------------------------------------------------------

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteModule(int id) // id, ModuleId'dir.
        {
            var moduleToDelete = _courseRepository.GetModuleById(id);
            if (moduleToDelete == null) return NotFound();

            var course = _courseRepository.GetCourseById(moduleToDelete.CourseId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Yetki Kontrolü: Sadece kursun sahibi silebilir.
            if (course == null || course.InstructorId != userId)
            {
                return Forbid();
            }

            // Silme işlemini gerçekleştir
            _courseRepository.DeleteModule(moduleToDelete);
            _courseRepository.Save();

            TempData["SuccessMessage"] = $"'{moduleToDelete.Title}' adlı modül başarıyla silindi.";

            // Modül Yönetimi sayfasına geri yönlendir
            return RedirectToAction(nameof(ManageModules), new { courseId = moduleToDelete.CourseId });
        }
    }
}