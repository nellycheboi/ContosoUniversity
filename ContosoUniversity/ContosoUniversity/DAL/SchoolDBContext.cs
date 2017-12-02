using ContosoUniversity.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace ContosoUniversity.DAL
{
    /// <summary>
    /// With entity the pluralized forms of entity class names are used as table names
    /// Entity property names are used as column names
    /// Entity properties that are named ID or classname ID are recognized as primary key properties.
    /// A property is interpreted as a foreign key property 
    /// if it's named <navigation property name><primary key property name> (for example, StudentID for the Student navigation property since the Student entity's primary key is ID)
    /// </summary>
    public class SchoolDBContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        // This two could be omitted. 
        // The Entity Framework would include them implicitly because the Student entity references the Enrollment entity and the Enrollment entity references the Course entity.
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        // he pluralized forms of entity class names to be used as table names.
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<PluralizingTableNameConvention>();
        }
    }
}