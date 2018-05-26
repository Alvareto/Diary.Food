namespace Diary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Type_to_Meal_and_Ingredient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ingredients", "IngredientType", c => c.Int(nullable: false));
            AddColumn("dbo.Meals", "MealType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Meals", "MealType");
            DropColumn("dbo.Ingredients", "IngredientType");
        }
    }
}
