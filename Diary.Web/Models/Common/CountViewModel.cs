using System.Collections.Generic;

namespace Diary.Web.Models.Common
{
    public class CountViewModel
    {
        public int Meals { get; set; }
        public int Ingredients { get; set; }
        public int NutritionFacts { get; set; }
        public int Users { get; set; }

        public int MealBreakfast { get; set; }
        public int MealLunch { get; set; }
        public int MealDinner { get; set; }
        public int MealSnack { get; set; }
        public int MealOther { get; set; }

        public int IngredientMeat { get; set; }
        public int IngredientVegetable { get; set; }
        public int IngredientFruit { get; set; }
        public int IngredientDrink { get; set; }
        public int IngredientOther { get; set; }

        public int MealToday { get; set; }
        public int MealYesterday { get; set; }
        public int MealLastWeek { get; set; }
        public int MealLastMonth { get; set; }

        public int TotalCalories { get; set; }
        public int TotalCarbs { get; set; }
        public int TotalFat { get; set; }
        public int TotalProtein { get; set; }

        public IReadOnlyList<string> MostUsedIngredients { get; set; }
    }
}