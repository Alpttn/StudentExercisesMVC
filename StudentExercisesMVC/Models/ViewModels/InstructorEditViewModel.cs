﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class InstructorEditViewModel
    {
        public Instructor Instructor { get; set; }
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
    }
}
