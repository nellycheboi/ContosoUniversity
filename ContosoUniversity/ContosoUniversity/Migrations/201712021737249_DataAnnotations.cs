namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataAnnotations : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Students", name: "FirstMidName", newName: "FirstName");
            AlterColumn("dbo.Students", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Students", "FirstName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Students", "FirstName", c => c.String());
            AlterColumn("dbo.Students", "LastName", c => c.String());
            RenameColumn(table: "dbo.Students", name: "FirstName", newName: "FirstMidName");
        }
    }
}
