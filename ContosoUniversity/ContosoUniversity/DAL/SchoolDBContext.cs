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
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        // This is all that the Entity Framework needs in order to configure table-per-hierarchy inheritance. As you'll see, when the database is updated, it will have a Person table in place of the Student and Instructor tables.
        public DbSet<Person> People { get; set; }
        // he pluralized forms of entity class names to be used as table names.
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<PluralizingTableNameConvention>();
            // For the many-to-many relationship between the Instructor and Course entities, the code specifies the table and column names for the join table. Code First can configure the many-to-many relationship for you without this code, but if you don't call it, you will get default names such as InstructorInstructorID for the InstructorID column.

            // Checkout https://blogs.msdn.microsoft.com/aspnetue/2011/05/04/entity-framework-code-first-tutorial-supplement-what-is-going-on-in-a-fluent-api-call/
            modelBuilder.Entity<Course>()
             .HasMany(c => c.Instructors).WithMany(i => i.Courses)
             .Map(t => t.MapLeftKey("ID")
                 .MapRightKey("InstructorID")
                 .ToTable("CourseInstructor"));


            modelBuilder.Entity<Instructor>()
                .HasOptional(p => p.OfficeAssignment).WithRequired(p => p.Instructor);
            modelBuilder.Entity<Department>().MapToStoredProcedures();

            //If you prefer to use the fluent API you can use the IsConcurrencyToken method to specify the tracking property
            modelBuilder.Entity<Department>()
            .Property(p => p.RowVersion).IsConcurrencyToken();
        }
    }

}