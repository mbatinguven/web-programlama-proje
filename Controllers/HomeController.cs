using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // ?? YÖNLENDÝRME 1: Adminler için Yönetim Kontrol Paneli (Zaten Hazýr)
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }

                // ?? YÖNLENDÝRME 2: Eðitmenler için özel Dashboard
                else if (User.IsInRole("Instructor"))
                {
                    // Yeni oluþturacaðýmýz Dashboard'a yönlendir
                    return View("InstructorDashboard");
                }

                // ?? YÖNLENDÝRME 3: Öðrenciler için özel Dashboard
                else if (User.IsInRole("Student"))
                {
                    // Yeni oluþturacaðýmýz Dashboard'a yönlendir
                    return View("StudentDashboard");
                }

                // Rolü yoksa, varsayýlan olarak Kurs listesine gönder.
                return RedirectToAction("Index", "Course");
            }

            // Misafir: Tanýtým sayfasýný göster (Mevcut Index View)
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
