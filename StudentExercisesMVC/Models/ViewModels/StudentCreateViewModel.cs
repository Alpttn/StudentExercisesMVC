using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class StudentCreateViewModel
    {
        public List<SelectListItem> Cohorts { get; set; }
        public Student Student { get; set; }
    }
}
