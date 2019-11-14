using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class StudentEditViewModel
    {
        public Student Student { get; set; }
        public List<Cohort> Cohorts { get; set; } = new List<Cohort>();

        public List<SelectListItem> CohortOptions
        {
            get
            {
                if (Cohorts == null) return null;

                return Cohorts
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                    .ToList();
            }
        }
        public IEnumerable<Exercise> Exercises { get; set; }

        public List<SelectListItem> ExerciseOptions
        {
            get
            {
                if (Exercises == null) return null;

                return Exercises
                    .Select(e => new SelectListItem(e.Name, e.Language.ToString()))
                    .ToList();
            }
        }
    }
}
