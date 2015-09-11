namespace skypebot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _something : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Uri = c.String(),
                        User_Handle = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Handle)
                .Index(t => t.User_Handle);
            
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
            DropForeignKey("dbo.Permissions", "User_Handle", "dbo.Users");
            DropIndex("dbo.Permissions", new[] { "User_Handle" });
            DropTable("dbo.Users");
            DropTable("dbo.Permissions");
        }
    }
}
