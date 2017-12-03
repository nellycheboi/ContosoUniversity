using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public class OfficeAssignment
    {
        /// <summary>
        /// There's a one-to-zero-or-one relationship between the Instructor and the OfficeAssignment entities. An office assignment only exists in relation to the instructor it's assigned to, and therefore its primary key is also its foreign key to the Instructor entity. But the Entity Framework can't automatically recognize InstructorID as the primary key of this entity because its name doesn't follow the ID or classname ID naming convention. Therefore, the Key attribute is used to identify it as the key:
        /// 
        /// When there is a one-to-zero-or-one relationship or a one-to-one relationship between two entities (such as between OfficeAssignment and Instructor), EF can't work out which end of the relationship is the principal and which end is dependent. One-to-one relationships have a reference navigation property in each class to the other class. The ForeignKey Attribute can be applied to the dependent class to establish the relationship. If you omit the ForeignKey Attribute, you get the following error when you try to create the migration:
        /// </summary>
        [Key]
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }

        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        /// <summary>
        /// The Instructor entity has a nullable OfficeAssignment navigation property (because an instructor might not have an office assignment), and the OfficeAssignment entity has a non-nullable Instructor navigation property (because an office assignment can't exist without an instructor -- InstructorID is non-nullable). When an Instructor entity has a related OfficeAssignment entity, each entity will have a reference to the other one in its navigation property.
        /// 
        /// You could put a [Required] attribute on the Instructor navigation property to specify that there must be a related instructor, but you don't have to do that because the InstructorID foreign key (which is also the key to this table) is non-nullable
        /// </summary>
        
        public virtual Instructor Instructor { get; set; }
    }
}