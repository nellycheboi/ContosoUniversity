namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inheritance : DbMigration
    {/// <summary>
     /// Removes foreign key constraints and indexes that point to the Student table.
     /// Renames the Instructor table as Person and makes changes needed for it to store Student data
     ///  Adds nullable EnrollmentDate for students.
     ///  Adds Discriminator column to indicate whether a row is for a student or an instructor.
     ///  Makes HireDate nullable since student rows won't have hire dates.
     ///  Adds a temporary field that will be used to update foreign keys that point to students.When you copy students into the Person table they'll get new primary key values.
     /// Copies data from the Student table into the Person table. This causes students to get assigned new primary key values.
     /// Fixes foreign key values that point to students.
     /// Re-creates foreign key constraints and indexes, now pointing them to the Person table.
     /// </summary>
        public override void Up()
        {
            // Drop foreign keys and indexes that point to tables we're going to drop.
            DropForeignKey("dbo.Enrollments", "StudentID", "dbo.Students");
            DropIndex("dbo.Enrollments", new[] { "StudentID" });

            RenameTable(name: "dbo.Instructors", newName: "People");
            AddColumn("dbo.People", "EnrollmentDate", c => c.DateTime());
            AddColumn("dbo.People", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue: "Instructor"));
            AlterColumn("dbo.People", "HireDate", c => c.DateTime());
            AddColumn("dbo.People", "OldId", c => c.Int(nullable: true));

            // Copy existing Student data into new Person table.
            Sql("INSERT INTO dbo.People (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) SELECT LastName, FirstName, null AS HireDate, EnrollmentDate, 'Student' AS Discriminator, ID AS OldId FROM dbo.Students");

            // Fix up existing relationships to match new PK's.
            Sql("UPDATE dbo.Enrollments SET StudentId = (SELECT ID FROM dbo.People WHERE OldId = Enrollments.StudentId AND Discriminator = 'Student')");

            // Remove temporary key
            DropColumn("dbo.People", "OldId");

            DropTable("dbo.Students");

            // Re-create foreign keys and indexes pointing to new table.
            AddForeignKey("dbo.Enrollments", "StudentID", "dbo.People", "ID", cascadeDelete: true);
            CreateIndex("dbo.Enrollments", "StudentID");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AlterColumn("dbo.People", "HireDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.People", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.People", "LastName", c => c.String(maxLength: 50));
            DropColumn("dbo.People", "Discriminator");
            DropColumn("dbo.People", "EnrollmentDate");
            RenameTable(name: "dbo.People", newName: "Instructors");
        }
    }
}
