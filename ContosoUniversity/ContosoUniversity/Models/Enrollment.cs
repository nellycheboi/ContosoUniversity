using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
 
        public int ID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Grade? Grade { get; set; }

        /// <summary>
        /// Has one course
        /// </summary>
        public virtual Course Course { get; set; }

        /// <summary>
        /// Has one student
        /// </summary>
        public virtual Student Student { get; set; }
    }
}