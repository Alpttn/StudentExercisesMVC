﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;


namespace StudentExercisesMVC.Models
{
    public class Instructor : NSSPerson
    {
        public int Id { get; set; }
        public string Speciality { get; set; }

        //public Instructor(string firstName, string lastName, string slackHandle, Cohort cohort, string instructorSpeciality)
        //{
        //    FirstName = firstName;
        //    LastName = lastName;
        //    SlackHandle = slackHandle;
        //    Cohort = cohort;
        //    InstructorSpeciality = instructorSpeciality;
        //}

        //public void assignExercise(Student student, Exercise exercise)
        //{
        //    student.exerciseList.Add(exercise);
        //}

    }
}
