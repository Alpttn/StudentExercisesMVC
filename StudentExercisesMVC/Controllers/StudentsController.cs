using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Students
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.id, s.firstname, s.lastname, 
                                               s.slackhandle, s.cohortId,
                                               c.name as cohortname
                                         FROM Student s INNER JOIN Cohort c on c.id = s.cohortid";
                    SqlDataReader reader = cmd.ExecuteReader();

                    var students = new List<Student>();
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cohortId")),
                                Name = reader.GetString(reader.GetOrdinal("cohortname")),
                            }
                        };

                        students.Add(student);
                    }

                    reader.Close();

                    return View(students);
                }
            }

        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId,
                                                c.Name AS CohortName  
                                         FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id
                                         WHERE s.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student aStudent = null;
                    if (reader.Read())
                    {
                        aStudent = new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };
                    }

                    reader.Close();
                    return View(aStudent);
                    
                }
            }
        }


        // GET: Students/Create
        [HttpGet]
        public ActionResult Create()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM Cohort";
                    var reader = cmd.ExecuteReader();

                    var cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        cohorts.Add(
                                new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                }
                            );
                    }

                    reader.Close();

                    var viewModel = new StudentCreateViewModel()
                    {
                        Cohorts = cohorts
                    };

                    return View(viewModel);
                }
            }
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentCreateViewModel viewModel)
        {
            try
            {
                var newStudent = viewModel.Student;

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId)
                                                VALUES (@firstName, @lastName, @slackHandle, @cohortId);";
                        cmd.Parameters.Add(new SqlParameter("@firstName", newStudent.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", newStudent.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", newStudent.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", newStudent.CohortId));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            var student = GetStudentById(id);
            var viewModel = new StudentEditViewModel()
            {
                //Student = GetStudentById(id),
                Student = student,
                Cohorts = GetAllCohorts(),
                //AllExercises = GetAllExercises(), create this helper method below
                //SelectedExerciseIds = GetStudentById(id).Exercises.Select(e => e.Id).ToList()
                SelectedExerciseIds = student.ExerciseList.Select(e => e.Id).ToList()
            };

            return View(viewModel);

        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, StudentEditViewModel viewModel)
        {
            var updatedStudent = viewModel.Student;
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            DELETE FROM StudentExercise WHERE StudentId = @id;

                            UPDATE Student 
                               SET FirstName = @firstName, 
                                   LastName = @lastName, 
                                   SlackHandle = @slackHandle, 
                                   CohortId = @cohortId
                            WHERE id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", updatedStudent.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", updatedStudent.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", updatedStudent.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", updatedStudent.CohortId));

                        cmd.ExecuteNonQuery();

                        //took this outside of the loop below bc we only need to do this once
                        cmd.CommandText = @"
                                 INSERT INTO StudentExercise (StudentId, ExerciseId)
                                    VALUES (@studentId, @exerciseId)";
                        foreach(var exerciseId in viewModel.SelectedExerciseIds)
                        {
                            cmd.Parameters.Clear(); //delete the params
                            cmd.Parameters.Add(new SqlParameter("@studentId", id));
                            cmd.Parameters.Add(new SqlParameter("@exerciseId", exerciseId));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                viewModel = new StudentEditViewModel()
                {
                    Student = updatedStudent,
                    Cohorts = GetAllCohorts()
                };

                return View(viewModel);
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            var student = GetStudentById(id);
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            DELETE FROM StudentExercise WHERE StudentId = @id;
                            DELETE FROM Student WHERE id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        //helper method private
        private Student GetStudentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) //add the new sql below
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId,
                                                c.Name AS CohortName 
                                                se.ExerciseId, e.Name AS ExerciseName, e.Language
                                         FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id
                                            LEFT JOIN StudentExercise
                                            LEFT JOIN
                                         WHERE s.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student aStudent = null;
                    while (reader.Read())
                    {
                        //we want to create this student one time
                        if (aStudent == null)
                        {
                            aStudent = new Student()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                }
                            };
                        }
                        //make sure the student has an exercise, if i have an exercise in the db then add that exercise
                        if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                        aStudent.ExerciseList.Add(
                            new Exercise()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ExcerciseId")),
                                Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                Language = reader.GetString(reader.GetOrdinal("Language")),
                            }
                            );
                    }

                    reader.Close();
                    return aStudent;

                }
            }
        }
        //helper method get list of cohorts
        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cohort";
                    SqlDataReader reader = cmd.ExecuteReader();

                    var cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        cohorts.Add(
                                new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                }
                            );
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }
        //helper method
        //public async Task<ActionResult> Create(StudentCreateViewModel model)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"INSERT INTO Student
        //                                ( FirstName, LastName, SlackHandle, CohortId )
        //                                VALUES
        //                                ( @firstName, @lastName, @slackHandle, @cohortId )";
        //            cmd.Parameters.Add(new SqlParameter("@firstName", model.Student.FirstName));
        //            cmd.Parameters.Add(new SqlParameter("@lastName", model.Student.LastName));
        //            cmd.Parameters.Add(new SqlParameter("@slackHandle", model.Student.SlackHandle));
        //            cmd.Parameters.Add(new SqlParameter("@cohortId", model.Student.CohortId));
        //            cmd.ExecuteNonQuery();

        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //}

    }

}