using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// . In the code for the Department entity, the Column attribute is being used to change SQL data type mapping so that the column will be defined using the SQL Server money type in the database:
        /// 
        /// Column mapping is generally not required, because the Entity Framework usually chooses the appropriate SQL Server data type based on the CLR type that you define for the property. The CLR decimal type maps to a SQL Server decimal type. But in this case you know that the column will be holding currency amounts, and the money data type is more appropriate for that. For more information about CLR data types and how they match to SQL Server data types, https://msdn.microsoft.com/en-us/library/bb896344.aspx
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The Timestamp attribute specifies that this column will be included in the Where clause of Update and Delete commands sent to the database. The attribute is called Timestamp because previous versions of SQL Server used a SQL timestamp data type before the SQL rowversion replaced it. The .Net type for rowversion is a byte array.
        /// 
        /// The data type of the tracking column is typically rowversion. The rowversion value is a sequential number that's incremented each time the row is updated. In an Update or Delete command, the Where clause includes the original value of the tracking column (the original row version). If the row being updated has been changed by another user, the value in the rowversion column is different than the original value, so the Update or Delete statement can't find the row to update because of the Where clause. When the Entity Framework finds that no rows have been updated by the Update or Delete command (that is, when the number of affected rows is zero), it interprets that as a concurrency conflict.
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }
        /// <summary>
        /// A department may or may not have an administrator, and an administrator is always an instructor. Therefore the InstructorID property is included as the foreign key to the Instructor entity, and a question mark is added after the int type designation to mark the property as nullable.The navigation property is named Administrator but holds an Instructor entity:
        /// 
        /// By convention, the Entity Framework enables cascade delete for non-nullable foreign keys and for many-to-many relationships. This can result in circular cascade delete rules, which will cause an exception when you try to add a migration. For example, if you didn't define the Department.InstructorID property as nullable, you'd get the following exception message: "The referential relationship will result in a cyclical reference that's not allowed." If your business rules required InstructorID property to be non-nullable, you would have to use the following fluent API statement to disable cascade delete on the relationship:
        /// modelBuilder.Entity().HasRequired(d => d.Administrator).WithMany().WillCascadeOnDelete(false);
        /// 
        /// zero-or-one-to-many relationship line (0..1 to *) between the Instructor and Department entities.
        /// </summary>
        public int? InstructorID { get; set; }

        public virtual Instructor Administrator { get; set; }
        /// <summary>
        /// A department may have many courses, so there's a Courses navigation property:
        /// </summary>
        public virtual ICollection<Course> Courses { get; set; }
    }
}