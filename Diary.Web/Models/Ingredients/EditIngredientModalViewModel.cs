using System.Collections.Generic;
using System.Linq;
using Diary.Domain.Dto;

namespace Diary.Web.Models.Ingredients
{
    public class EditIngredientModalViewModel
    {
        public IngredientDto Ingredient { get; set; }

        public IReadOnlyList<NutritionFactDto> NutritionFacts { get; set; }

        public bool IsIngredientFact(NutritionFactDto fact)
        {
            return Ingredient.NutritionFacts != null && Ingredient.NutritionFacts.Any(r => r.Id == fact.Id);
        }
    }
}