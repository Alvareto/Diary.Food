using System.Collections.Generic;
using System.Linq;
using Diary.Domain.Dto;

namespace Diary.Web.Models.Meals
{
    public class EditMealModalViewModel
    {
        public MealDto Meal { get; set; }

        public IReadOnlyList<IngredientDto> Ingredients { get; set; }

        public bool MealHasIngredient(IngredientDto ingredient)
        {
            return Meal.Ingredients != null && Meal.Ingredients.Any(r => r.Name == ingredient.Name);
        }
    }
}