using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public class Instructor
    {
        public int ID { get; set; }

        [Display(Name = "Last Name"), StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }

        [Column("FirstName"), Display(Name = "First Name"), StringLength(50, MinimumLength = 1)]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date), Display(Name = "Hire Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstMidName; }
        }

        /// <summary>
        /// The Courses and OfficeAssignment properties are navigation properties. As was explained earlier, they are typically defined as virtual so that they can take advantage of an Entity Framework feature called lazy loading. In addition, if a navigation property can hold multiple entities, its type must implement the ICollection<T> Interface. For example IList<T> qualifies but not IEnumerable<T> because IEnumerable<T> doesn't implement Add.
      ///  An instructor can teach any number of courses, so Courses is defined as a collection of Course entities.
        /// </summary>
        public virtual ICollection<Course> Courses { get; set; }

        /// <summary>
        /// The Instructor entity has a nullable OfficeAssignment navigation property (because an instructor might not have an office assignment), and the OfficeAssignment entity has a non-nullable Instructor navigation property (because an office assignment can't exist without an instructor -- InstructorID is non-nullable). When an Instructor entity has a related OfficeAssignment entity, each entity will have a reference to the other one in its navigation property.
        /// 
        /// one-to-zero-or-one relationship line (1 to 0..1) between the Instructor and OfficeAssignment
        /// </summary>
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}