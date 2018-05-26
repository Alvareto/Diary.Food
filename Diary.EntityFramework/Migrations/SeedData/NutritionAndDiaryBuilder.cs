using System.Collections.Generic;
using System.Linq;
using Diary.Domain.Models;
using Diary.EntityFramework;
using Diary.MultiTenancy;

namespace Diary.Migrations.SeedData
{
    public class DefaultNutritionAndDiaryBuilder
    {
        private readonly DiaryDbContext _context;

        public DefaultNutritionAndDiaryBuilder(DiaryDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultIngredients();
            CreateDefaultDiary();
        }

        //private List<NutritionFact> CreateDefaultNutritionFacts()
        //{
        //    return Ingredient.GetDefaultNutritionFacts();
        //}

        public static Dictionary<string, IngredientType> SeedIngredientMap = new Dictionary<string, IngredientType>
        {
            { "Chicken", IngredientType.Meat},
            { "Turkey", IngredientType.Meat},
            { "Pig", IngredientType.Meat},
            { "Cucumber", IngredientType.Vegetable},
            { "Tomato", IngredientType.Vegetable},
            { "Green Salad", IngredientType.Vegetable},
            { "Water", IngredientType.Drink},
        };

        public static List<string> SeedIngredientList = new List<string>()
        {
            "Chicken",
            "Turkey",
            "Pig",
            "Cucumber",
            "Tomato",
            "Green Salad",
            "Water"
        };

        public static List<string> SeedMealList = new List<string>()
        {
            "Ceaser's Salad",
            "Fillet Mignon",
            "Steak"
        };

        public static Dictionary<string, List<string>> SeedMealIngredientsMap = new Dictionary<string, List<string>>()
        {
            { "Ceaser's Salad", new List<string>() {"Cucumber", "Tomato", "Green Salad"}},
            { "Fillet Mignon", new List<string>() {"Chicken", "Cucumber"}},
            { "Steak", new List<string>() {"Pig"}}
        };

        private void CreateDefaultIngredients()
        {
            var db = _context.Ingredients.Select(i => i.Name).ToList();//.Any(t => DiaryManager.SeedIngredientList.Contains(t.Name));
            var web = SeedIngredientMap;

            foreach (var nameType in web)
            {
                var name = nameType.Key;
                var type = nameType.Value;

                if (db.Any(i => i == name)) continue; // if it's already in db

                var facts = Ingredient.GetDefaultNutritionFacts();
                foreach (var fact in facts)
                {
                    _context.NutritionFacts.Add(fact);
                }

                var ingredient = Ingredient.Create(name, type, facts);
                _context.Ingredients.Add(ingredient);
            }

            _context.SaveChanges();
        }

        private void CreateDefaultDiary()
        {
            var db = _context.Meals.Select(m => m.Name).ToList();
            var web = SeedMealIngredientsMap;
            var adminUser = _context.Users.First();

            foreach (var kv in web)
            {
                var name = kv.Key;
                if (db.Any(m => m == name)) continue; // if it's already in db

                var ingredients = _context.Ingredients.Where(i => kv.Value.Contains(i.Name)).ToList();

                var meal = Meal.Create(name);

                meal.AsUser(adminUser);
                meal.AddIngredients(ingredients);

                _context.Meals.Add(meal);
            }

            _context.SaveChanges();
        }
    }
}
