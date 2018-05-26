using System;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Diary.Domain.Models;

namespace Diary.Domain
{
    public interface ICountAppService : IApplicationService
    {
        IHasTotalCount GetCountMeals();
        IHasTotalCount GetCountMeals(DateTime dateFrom);
        IHasTotalCount GetCountMealType(MealType type);
        IHasTotalCount GetCountIngredients();
        IHasTotalCount GetCountIngredientType(IngredientType type);
        ListResultDto<string> GetMostUsedIngredients(int count);
        IHasTotalCount GetCountNutritionFacts();
        IHasTotalCount GetCountNutritionFactType(Nutrient type);
        IHasTotalCount GetCountUsers();
    }
}