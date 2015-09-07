namespace repostpolice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Handle = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Handle);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
