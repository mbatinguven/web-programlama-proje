using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.Repository;
using WebProgramlamaProje.Models; // ApplicationUser'ı kullanmak için eklendi

namespace WebProgramlamaProje
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // 💡 Veritabanı Düzeltme: SQLite kullanıyoruz.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // 1. Düzeltme: Identity Servis Kaydı (IdentityUser -> ApplicationUser)
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                 .AddRoles<IdentityRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>();

            // 💡 ZORUNLU EKLEME: Repository Pattern Kaydı
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Seed Data Bloğu
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // 2. Düzeltme: Seed Data Bloğu (UserManager<IdentityUser> -> UserManager<ApplicationUser>)
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // 💡 SeedData çalıştırılıyor.
                WebProgramlamaProje.Data.SeedData.Initialize(roleManager, userManager).Wait();
            }
            app.Run();
        }
    }
}