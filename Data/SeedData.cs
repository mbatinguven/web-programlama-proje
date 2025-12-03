// Data/SeedData.cs
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using System.Linq;
using WebProgramlamaProje.Models; // ApplicationUser için gerekli

namespace WebProgramlamaProje.Data
{
    public static class SeedData
    {
        public static readonly string[] Roles = { "Admin", "Instructor", "Student" };

        private const string AdminUserEmail = "admin@proje.com";
        private const string AdminPassword = "Admin123*";

        private const string TestUserEmail = "mbg@proje.com";
        private const string TestUserPassword = "Test123*";

        private const string StudentUserEmail = "student@proje.com";
        private const string StudentPassword = "Student123*"; // Yeni öğrenci parolası
        // 💡 DÜZELTME: UserManager tipini ApplicationUser olarak güncelliyoruz.
        public static async Task Initialize(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // 1. Zorunlu Rolleri Oluşturma
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // --- ADMIN KULLANICISI (SADECE Admin Rolü) ---
            var adminUser = await userManager.FindByEmailAsync(AdminUserEmail);
            if (adminUser == null)
            {
                // 💡 DÜZELTME: IdentityUser yerine ApplicationUser nesnesi oluşturuluyor.
                adminUser = new ApplicationUser { UserName = AdminUserEmail, Email = AdminUserEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, AdminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Hata oluşursa fırlat, uygulamanın çökmesini engelle.
                    throw new InvalidOperationException($"Admin kullanıcısı oluşturulamadı: {result.Errors.First().Description}");
                }
            }

            // --- TEST KULLANICISI (TÜM ROLLER) ---
            var testUser = await userManager.FindByEmailAsync(TestUserEmail);

            if (testUser == null)
            {
                // 💡 DÜZELTME: IdentityUser yerine ApplicationUser nesnesi oluşturuluyor.
                testUser = new ApplicationUser { UserName = TestUserEmail, Email = TestUserEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(testUser, TestUserPassword);

                if (result.Succeeded)
                {
                    // Yeni kullanıcıya tüm rolleri (Admin, Instructor, Student) ata
                    foreach (var role in Roles)
                    {
                        await userManager.AddToRoleAsync(testUser, role);
                    }
                }
                else
                {
                    // Hata oluşursa fırlat
                    throw new InvalidOperationException($"Test kullanıcısı oluşturulamadı: {result.Errors.First().Description}");
                }
            }

            var studentUser = await userManager.FindByEmailAsync(StudentUserEmail);

            if (studentUser == null)
            {
                studentUser = new ApplicationUser { UserName = StudentUserEmail, Email = StudentUserEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(studentUser, StudentPassword);

                if (result.Succeeded)
                {
                    // 💡 YENİ EKLEME: SADECE Student rolü atandı.
                    await userManager.AddToRoleAsync(studentUser, "Student");
                }
                else
                {
                    throw new InvalidOperationException($"Öğrenci kullanıcısı oluşturulamadı: {result.Errors.First().Description}");
                }
            }
        }
    }
}