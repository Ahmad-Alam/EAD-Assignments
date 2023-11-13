using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
#nullable disable

namespace MVC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var context = new MyContext();

            // Applying all Editing Rights to the Database
            // Grant update, alter, and delete permissions to the Students table
            context.Database.ExecuteSqlCommand("GRANT UPDATE, ALTER, DELETE ON [MVC.Program+MyContext].dbo.Students TO [Alam\\user1]");
            // Grant update, alter, and delete permissions to the Courses table
            context.Database.ExecuteSqlCommand("GRANT UPDATE, ALTER, DELETE ON [MVC.Program+MyContext].dbo.Courses TO [Alam\\user1]");
            // Grant update, alter, and delete permissions to the Enrollments table
            context.Database.ExecuteSqlCommand("GRANT UPDATE, ALTER, DELETE ON [MVC.Program+MyContext].dbo.Enrollments TO [Alam\\user1]");

            // Create and save new students
            Console.WriteLine("Adding new students");
            var transaction = context.Database.BeginTransaction();

            var student = new Student
            {
                FirstName = "Ahmad",
                LastName = "Alam",
                EnrollmentDate = DateTime.Parse(DateTime.Today.ToString()),
                DateOfBirth = DateOnly.Parse("2002-07-22"),
                Email = "bcsf20a003@pucit.edu.pk"
            };

            context.Students.Add(student);

            var student1 = new Student
            {
                FirstName = "Ali",
                LastName = "Ahmed",
                EnrollmentDate = DateTime.Parse(DateTime.Today.ToString()),
                DateOfBirth = DateOnly.Parse("2002-07-22"),
                Email = "text@pucit.edu.pk"
            };

            // Additional dummy data
            var student2 = new Student
            {
                FirstName = "John",
                LastName = "Doe",
                EnrollmentDate = DateTime.Parse("2022-01-01"),
                DateOfBirth = DateOnly.Parse("2000-01-01"),
                Email = "john.doe@example.com"
            };

            var student3 = new Student
            {
                FirstName = "Jane",
                LastName = "Smith",
                EnrollmentDate = DateTime.Parse("2022-02-01"),
                DateOfBirth = DateOnly.Parse("1999-02-15"),
                Email = "jane.smith@example.com"
            };

            context.Students.Add(student2);
            context.Students.Add(student3);

            context.SaveChanges();


            context.Students.Add(student1);
            context.SaveChanges();

            var students = (from s in context.Students
                            orderby s.FirstName
                            select s).ToList<Student>();

            Console.WriteLine("Retrieve all students from the database:");

            foreach (var stdnt in students)
            {
                string name = stdnt.FirstName + " " + stdnt.LastName;
                Console.WriteLine("ID: {0}, Name: {1}, Email: {2}, DateOfBirth: {3}", stdnt.ID, name, stdnt.Email, stdnt.DateOfBirth);
            }

            Console.WriteLine("Trying to update the students where the first name is Ali:");
            string updateQuery = "UPDATE [MVC.Program+MyContext].dbo.Students SET FirstName = 'Hamza' WHERE FirstName = 'Ali'";

            // Execute the SQL query
            context.Database.ExecuteSqlCommand(updateQuery);
            context.SaveChanges();
            transaction.Commit();

            // Force Entity Framework to reload the entities from the database
            context.Entry(student1).Reload();
            Console.WriteLine("Students updated successfully!");

            students = (from s in context.Students
                        orderby s.FirstName
                        select s).ToList<Student>();

            Console.WriteLine("Retrieve all students from the database:");

            foreach (var stdnt in students)
            {
                string name = stdnt.FirstName + " " + stdnt.LastName;
                Console.WriteLine("ID: {0}, Name: {1}, Email: {2}, DateOfBirth: {3}", stdnt.ID, name, stdnt.Email, stdnt.DateOfBirth);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public enum Grade
        {
            A, B, C, D, F
        }

        public class Enrollment
        {
            public Enrollment()
            {
                Course = new Course();
                Student = new Student();
            }

            public int EnrollmentID { get; set; }
            public int CourseID { get; set; }
            public int StudentID { get; set; }
            public Grade? Grade { get; set; }

            public virtual Course Course { get; set; }
            public virtual Student Student { get; set; }
        }

        public class Student
        {
            public Student()
            {
                Enrollments = new List<Enrollment>();
                LastName = "";
                FirstName = "";
                Email = "";
            }

            public int ID { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Email { get; set; }
            public DateTime EnrollmentDate { get; set; }
            public DateOnly DateOfBirth { get; set; }
            public virtual ICollection<Enrollment> Enrollments { get; set; }
        }

        public class Course
        {
            public Course()
            {
                Enrollments = new List<Enrollment>();
                Title = "";
            }

            public int CourseID { get; set; }
            public string Title { get; set; }
            public int Credits { get; set; }

            public virtual ICollection<Enrollment> Enrollments { get; set; }
        }

        public class MyContext : DbContext
        {
            public virtual DbSet<Course> Courses { get; set; } = null!;
            public virtual DbSet<Enrollment> Enrollments { get; set; } = null!;
            public virtual DbSet<Student> Students { get; set; } = null!;
        }
    }
}
