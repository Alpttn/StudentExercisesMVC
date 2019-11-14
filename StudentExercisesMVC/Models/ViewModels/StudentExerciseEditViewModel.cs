using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class StudentExerciseEditViewModel
    {
        public Student Student { get; set; }
        //public List<Exercise> Exercises { get; set; } = new List<Exercise>();
        public MultiSelectList Exercises { get; set; }

        //public List<SelectListItem> CohortOptions
        //{
        //    get
        //    {
        //        if (Cohorts == null) return null;

        //        return Cohorts
        //            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
        //            .ToList();
        //    }
        //}
    }
}
