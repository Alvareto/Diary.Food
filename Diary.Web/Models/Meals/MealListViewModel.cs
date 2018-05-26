using System;
using System.Collections.Generic;
using Diary.Domain;
using Diary.Roles.Dto;

namespace Diary.Web.Models.Meals
{
    public class MealListViewModel
    {
        public DateTime Day { get; set; }
        public IReadOnlyList<MealDto> Meals { get; set; }

        public IReadOnlyList<IngredientDto> Ingredients { get; set; }
    }
}