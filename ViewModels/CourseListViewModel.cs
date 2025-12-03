// ViewModels/CourseListViewModel.cs

using WebProgramlamaProje.Models;
using System.Collections.Generic;

namespace WebProgramlamaProje.ViewModels
{
    public class CourseListViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
    }
}