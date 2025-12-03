// Data/ApplicationDbContext.cs

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebProgramlamaProje.Models; // Tüm Entity'lerimiz bu namespace'te
using System.Security.Claims;

namespace WebProgramlamaProje.Data
{
    // ApplicationUser'dan kalıtım alarak Identity'yi özel kullanıcı modelimize bağlarız.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Kendi Entity'lerimiz için DbSet'ler (Ödev Veri Modeli) ---

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<InstructorApplication> InstructorApplications { get; set; }

        // İlişkileri ve kısıtlamaları yapılandırmak için OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 💡 1. Course ve Instructor (ApplicationUser) İlişkisi:
            // Course'un bir Instructor'ı (Eğitmen) vardır.
            builder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.CoursesTaught) // ApplicationUser'daki CoursesTaught koleksiyonuna bağlı
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict); // Kurs silinse bile Eğitmeni koru (veya set null). Restrict en güvenlisidir.

            // 💡 2. Enrollment İlişkileri:
            // Enrollment (Kayıt) ve Student (ApplicationUser) İlişkisi
            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments) // ApplicationUser'daki Enrollments koleksiyonuna bağlı
                .HasForeignKey(e => e.StudentId);

            // Enrollment ve Course İlişkisi
            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            // İstenirse composite key tanımı buraya eklenebilir.


        }
    }
}