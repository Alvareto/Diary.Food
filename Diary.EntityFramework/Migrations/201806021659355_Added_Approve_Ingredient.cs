namespace Diary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Approve_Ingredient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ingredients", "IngredientStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Ingredients", "ApproverUser_Id", c => c.Long());
            CreateIndex("dbo.Ingredients", "ApproverUser_Id");
            AddForeignKey("dbo.Ingredients", "ApproverUser_Id", "dbo.AbpUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ingredients", "ApproverUser_Id", "dbo.AbpUsers");
            DropIndex("dbo.Ingredients", new[] { "ApproverUser_Id" });
            DropColumn("dbo.Ingredients", "ApproverUser_Id");
            DropColumn("dbo.Ingredients", "IngredientStatus");
        }
    }
}
