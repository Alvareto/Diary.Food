using System.Web.Mvc;
using Abp.Timing;
using Abp.Web.Mvc.Authorization;
using Diary.Domain;
using Diary.Domain.Models;
using Diary.Web.Models.Common;

namespace Diary.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : DiaryControllerBase
    {
        protected readonly ICountAppService _countService;

        public HomeController(ICountAppService countService)
        {
            _countService = countService;
        }

        public ActionResult Index()
        {
            var vm = new CountViewModel()
            {
                Meals = _countService.GetCountMeals().TotalCount,
                Ingredients = _countService.GetCountIngredients().TotalCount,
                NutritionFacts = _countService.GetCountNutritionFacts().TotalCount,
                Users = _countService.GetCountUsers().TotalCount,

                MealBreakfast = _countService.GetCountMealType(MealType.Breakfast).TotalCount,
                MealLunch = _countService.GetCountMealType(MealType.Lunch).TotalCount,
                MealDinner = _countService.GetCountMealType(MealType.Dinner).TotalCount,
                MealSnack = _countService.GetCountMealType(MealType.Snack).TotalCount,
                MealOther = _countService.GetCountMealType(MealType.Other).TotalCount,

                IngredientMeat = _countService.GetCountIngredientType(IngredientType.Meat).TotalCount,
                IngredientVegetable = _countService.GetCountIngredientType(IngredientType.Vegetable).TotalCount,
                IngredientFruit = _countService.GetCountIngredientType(IngredientType.Fruit).TotalCount,
                IngredientDrink = _countService.GetCountIngredientType(IngredientType.Drink).TotalCount,
                IngredientOther = _countService.GetCountIngredientType(IngredientType.Other).TotalCount,

                MealToday = _countService.GetCountMeals(Clock.Now.Date).TotalCount,
                MealYesterday = _countService.GetCountMeals(Clock.Now.Date.AddDays(-1)).TotalCount,
                MealLastWeek = _countService.GetCountMeals(Clock.Now.Date.AddDays(-7)).TotalCount,
                MealLastMonth = _countService.GetCountMeals(Clock.Now.Date.AddMonths(-1)).TotalCount,

                TotalCalories = _countService.GetCountNutritionFactType(Nutrient.Calories).TotalCount,
                TotalCarbs = _countService.GetCountNutritionFactType(Nutrient.Carbohydrates).TotalCount,
                TotalFat = _countService.GetCountNutritionFactType(Nutrient.Fat).TotalCount,
                TotalProtein = _countService.GetCountNutritionFactType(Nutrient.Protein).TotalCount,

                MostUsedIngredients = _countService.GetMostUsedIngredients(6).Items
            };

            return View(vm);
        }
    }
}