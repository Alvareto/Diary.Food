using System;
using System.Data.Entity;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Diary.Authorization.Users;
using Diary.Domain.Models;

namespace Diary.Domain
{
    public class CountAppService : DiaryAppServiceBase, ICountAppService
    {
        protected readonly IRepository<Meal> _mealRepository;
        protected readonly IRepository<Ingredient> _ingredientRepository;
        protected readonly IRepository<NutritionFact> _factRepository;
        protected readonly IRepository<User, long> _userRepository;

        public CountAppService(
            IRepository<Meal> mealRepository,
            IRepository<Ingredient> ingredientRepository,
            IRepository<NutritionFact> factRepository,
            IRepository<User, long> userRepository)
        {
            _mealRepository = mealRepository;
            _ingredientRepository = ingredientRepository;
            _factRepository = factRepository;
            _userRepository = userRepository;
        }

        public IHasTotalCount GetCountMeals()
        {
            return new CountDto { TotalCount = _mealRepository.Count() };
        }

        public IHasTotalCount GetCountMeals(DateTime dateFrom)
        {
            return new CountDto { TotalCount = _mealRepository.Count(m => m.Date >= dateFrom) };
        }

        public IHasTotalCount GetCountMealType(MealType type)
        {
            return new CountDto { TotalCount = _mealRepository.Count(m => m.Type == type) };
        }

        public IHasTotalCount GetCountIngredients()
        {
            return new CountDto { TotalCount = _ingredientRepository.Count() };
        }

        public IHasTotalCount GetCountIngredientType(IngredientType type)
        {
            return new CountDto { TotalCount = _ingredientRepository.Count(i => i.Type == type) };
        }

        public ListResultDto<string> GetMostUsedIngredients(int count)
        {
            var lst = _ingredientRepository.GetAll().Include(i => i.Meals)
                .OrderByDescending(i => i.Meals.Count)
                .Select(i => i.Name)
                .Take(count);

            return new ListResultDto<string>(lst.ToList());
        }

        public IHasTotalCount GetCountNutritionFacts()
        {
            return new CountDto { TotalCount = _factRepository.Count() };
        }

        public IHasTotalCount GetCountNutritionFactType(Nutrient type)
        {
            return new CountDto { TotalCount = _factRepository.GetAll().Where(i => i.Nutrient == type).Sum(i => i.Value) };
        }

        public IHasTotalCount GetCountUsers()
        {
            return new CountDto { TotalCount = _userRepository.Count() };
        }

        protected class CountDto : IHasTotalCount
        {
            public int TotalCount { get; set; }
        }
    }
}