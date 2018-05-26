using System;
using System.Collections.Generic;
using Diary.Domain;
using Diary.Roles.Dto;

namespace Diary.Web.Models.Ingredients
{
    public class IngredientListViewModel
    {
        public IReadOnlyList<IngredientDto> Ingredients { get; set; }

        public IReadOnlyList<NutritionFactDto> NutritionFacts { get; set; }
    }
}