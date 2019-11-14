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
        public IEnumerable<Exercise> AllExercises { get; set; } = new List<Exercise>();
        public List<int> SelectedExerciseIds { get; set; } = new List<int>(); //whats in the db right now. 

        public List<SelectListItem> ExerciseOptions
        {
            get
            {
                if (AllExercises == null) return null;

                return AllExercises
                    .Select(e => new SelectListItem($"{e.Name} ({e.Language})", e.Id.ToString()))
                    .ToList();
            }
        }
    }
}
