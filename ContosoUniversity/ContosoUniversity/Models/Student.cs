using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50,MinimumLength = 1, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        /// <summary>
        /// The DataType attribute is used to specify a data type that is more specific than the database intrinsic type. In this case we only want to keep track of the date, not the date and time
        /// The DataType attribute can also enable the application to automatically provide type-specific features. For example, a mailto: link can be created for DataType.EmailAddress, and a date selector can be provided for DataType.Date in browsers that support HTML5. The DataType attributes emits HTML 5 data- (pronounced data dash) attributes that HTML 5 browsers can understand. The DataType attributes do not provide any validation.
        /// If you use the DataType attribute with a date field, you have to specify the DisplayFormat attribute also in order to ensure that the field renders correctly in Chrome browsers. 
        /// http://stackoverflow.com/questions/12633471/mvc4-datatype-date-editorfor-wont-display-date-value-in-chrome-fine-in-ie
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

        /// <summary>
        /// The Enrollments property is a navigation property. Navigation properties hold other entities that are related to this entity. 
        /// In this case, the Enrollments property of a Student entity will hold all of the Enrollment entities that are related to that Student entity.
        /// In other words, if a given Student row in the database has two related Enrollment rows (rows that contain that student's primary key value in their StudentID foreign key column), 
        /// that Student entity's Enrollments navigation property will contain those two Enrollment entities
        /// Navigation properties are typically defined as virtual so that they can take advantage of certain Entity Framework functionality such as lazy loading.
        /// https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application
        /// To disable lazy loading, omit the virtual keyword.
        /// https://msdn.microsoft.com/en-US/data/jj574232
        /// 
        /// For all navigation properties, set LazyLoadingEnabled to false, put the following code in the constructor of your context class:
        /// 
        /// this.Configuration.LazyLoadingEnabled = false;
        /// one-to-many
        /// </summary>
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}