using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }
    /// <summary>
    /// There's a many-to-many relationship between the Student and Course entities, and the Enrollment entity functions as a many-to-many join table with payload in the database. This means that the Enrollment table contains additional data besides foreign keys for the joined tables (in this case, a primary key and a Grade property).
    /// 
    /// If the Enrollment table didn't include grade information, it would only need to contain the two foreign keys CourseID and StudentID. In that case, it would correspond to a many-to-many join table without payload (or a pure join table) in the database, and you wouldn't have to create a model class for it at all. The Instructor and Course entities have that kind of many-to-many relationship and therefore no entity class between them
    /// </summary>
    public class Enrollment
    {
 
        public int ID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }

        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        /// <summary>
        /// Has one course
        /// a course has many enrollments
        /// 
        /// </summary>
        public virtual Course Course { get; set; }

        /// <summary>
        /// Has one student
        /// students will have many enrollments too
        /// </summary>
        public virtual Student Student { get; set; }
    }
}