namespace Diary.Migrations
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Diary_Nutrition_Models : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Ingredient_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Meals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        MealDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(nullable: false),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Meal_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpUsers", t => t.CreatorUserId, cascadeDelete: true)
                .Index(t => t.CreatorUserId);
            
            CreateTable(
                "dbo.NutritionFacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nutrient = c.Int(nullable: false),
                        Value = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        Ingredient_Id = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_NutritionFact_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_Id)
                .Index(t => t.Ingredient_Id);
            
            CreateTable(
                "dbo.MealIngredients",
                c => new
                    {
                        Meal_Id = c.Int(nullable: false),
                        Ingredient_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Meal_Id, t.Ingredient_Id })
                .ForeignKey("dbo.Meals", t => t.Meal_Id, cascadeDelete: true)
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_Id, cascadeDelete: true)
                .Index(t => t.Meal_Id)
                .Index(t => t.Ingredient_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NutritionFacts", "Ingredient_Id", "dbo.Ingredients");
            DropForeignKey("dbo.Meals", "CreatorUserId", "dbo.AbpUsers");
            DropForeignKey("dbo.MealIngredients", "Ingredient_Id", "dbo.Ingredients");
            DropForeignKey("dbo.MealIngredients", "Meal_Id", "dbo.Meals");
            DropIndex("dbo.MealIngredients", new[] { "Ingredient_Id" });
            DropIndex("dbo.MealIngredients", new[] { "Meal_Id" });
            DropIndex("dbo.NutritionFacts", new[] { "Ingredient_Id" });
            DropIndex("dbo.Meals", new[] { "CreatorUserId" });
            DropTable("dbo.MealIngredients");
            DropTable("dbo.NutritionFacts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_NutritionFact_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Meals",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Meal_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Ingredients",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Ingredient_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
