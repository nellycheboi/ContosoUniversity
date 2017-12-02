using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }


        public DateTime EnrollmentDate { get; set; }

        /// <summary>
        /// The Enrollments property is a navigation property. Navigation properties hold other entities that are related to this entity. 
        /// In this case, the Enrollments property of a Student entity will hold all of the Enrollment entities that are related to that Student entity.
        /// In other words, if a given Student row in the database has two related Enrollment rows (rows that contain that student's primary key value in their StudentID foreign key column), 
        /// that Student entity's Enrollments navigation property will contain those two Enrollment entities
        /// Navigation properties are typically defined as virtual so that they can take advantage of certain Entity Framework functionality such as lazy loading.
        /// https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application
        /// 
        /// one-to-many
        /// </summary>
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}