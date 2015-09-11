namespace skypebot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mm_migration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions", "User_Handle", "dbo.Users");
            DropIndex("dbo.Permissions", new[] { "User_Handle" });
            CreateTable(
                "dbo.UserPermissions",
                c => new
                    {
                        Handle = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Handle, t.Id })
                .ForeignKey("dbo.Users", t => t.Handle, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.Id, cascadeDelete: true)
                .Index(t => t.Handle)
                .Index(t => t.Id);
            
            DropColumn("dbo.Permissions", "User_Handle");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Permissions", "User_Handle", c => c.String(maxLength: 128));
            DropForeignKey("dbo.UserPermissions", "Id", "dbo.Permissions");
            DropForeignKey("dbo.UserPermissions", "Handle", "dbo.Users");
            DropIndex("dbo.UserPermissions", new[] { "Id" });
            DropIndex("dbo.UserPermissions", new[] { "Handle" });
            DropTable("dbo.UserPermissions");
            CreateIndex("dbo.Permissions", "User_Handle");
            AddForeignKey("dbo.Permissions", "User_Handle", "dbo.Users", "Handle");
        }
    }
}
