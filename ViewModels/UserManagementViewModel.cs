// ViewModels/UserManagementViewModel.cs
using WebProgramlamaProje.Models;
using System.Collections.Generic;

namespace WebProgramlamaProje.ViewModels
{
    public class UserManagementViewModel
    {
        public List<ApplicationUser> Instructors { get; set; } = new List<ApplicationUser>();
        public List<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();
        public List<ApplicationUser> Admins { get; set; } = new List<ApplicationUser>();
    }
}