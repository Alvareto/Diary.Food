#define WRONG
//#undef WRONG

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Validation;
using Abp.UI;
using Diary.Authorization.Users;
using Diary.Domain.Dto;
using Diary.Domain.Models;

namespace Diary.Domain
{
    public class MealAppService : DiaryAppServiceBase<Meal, MealDto>, IMealAppService// ICrudAppService<Meal, MealDto>
    {
        protected readonly IRepository<Ingredient> _ingredientRepository;

        public MealAppService(IRepository<Meal> repository, IRepository<Ingredient> ingredientRepository)
            : base(repository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public override async Task<MealDto> Get(EntityDto<int> input)
        {
            CheckGetPermission();

            var meal = await Repository.GetAllIncluding(m => m.Ingredients.Select(i => i.NutritionFacts)).SingleOrDefaultAsync(m => m.Id == input.Id);

            if (meal == null)
            {
                throw new UserFriendlyException(404, "Meal not found with ID: " + input.Id);
            }

            return MapToEntityDto(meal);
        }

        public override async Task<PagedResultDto<MealDto>> GetAll(PagedAndSortedResultRequestDto input)
        {
            CheckGetAllPermission();

            var meals = await Repository.GetAllIncluding(m => m.Ingredients.Select(i => i.NutritionFacts)).ToListAsync();

            return new PagedResultDto<MealDto>(input.MaxResultCount, MapToEntityDtoList(meals)); //ObjectMapper.Map<List<MealDto>>(meals)
        }

        public override async Task<MealDto> Create(MealDto input)
        {
            CheckCreatePermission();

            //we can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a meal for input: " + input);

            // Creating a new Meal entity with given input's properties
            var meal = MapToEntity(input);

            // Saving entity with standard inset method of repositories
            var result = await Repository.InsertAsync(meal);

            return MapToEntityDto(result);
        }

        public async Task<MealDto> CreateWithNames(CreateMealDto input)
        {
            CheckCreatePermission();
            //we can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a meal for input: " + input);

            var user = await GetCurrentUserAsync();

            // Creating a new Meal entity with given input's properties
            var meal = MapToEntity(input, user);

            // Saving entity with standard insert method of repositories
            var result = await Repository.InsertAsync(meal);
            var map = MapToEntityDto(result);
            return map;

        }

        public override async Task<MealDto> Update(MealDto input)
        {
            CheckUpdatePermission();
            // we can use Logger, it's defined in ApplicationService base class.
            Logger.Info("Updating a task for input: " + input);

            // retrieving a meal entity with given id using standard Get method of repositories.
            var meal = await Repository.GetAsync(input.Id);

            // Updating changed properties of the retrieved task entity.
            MapToEntity(input, meal);

            return MapToEntityDto(meal); // Mapper.Map<MealDto>(meal);
        }

        public async Task<MealDto> UpdateWithNames(UpdateMealDto input)
        {
            CheckUpdatePermission();
            //we can use Logger, it's defined in ApplicationService class.
            Logger.Info("Updating a meal for input: " + input);

            // retrieving a meal entity with given id using standard Get method of repositories.
            var meal = await Repository.GetAsync(input.Id);

            // Updating changed properties of the retrieved task entity.
            MapToEntity(input, meal);

            return MapToEntityDto(meal);
        }

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var meal = await Repository.GetAsync(input.Id);
            meal.RemoveIngredients();

            await Repository.DeleteAsync(meal);
        }

        #region VALIDATIONS

        /// <inheritdoc />
        protected override Meal MapToEntity(MealDto createInput)
        {
            return base.MapToEntity(createInput);
        }

        protected Meal MapToEntity(CreateMealDto createInput, User u)
        {
            var meal = Meal.Create(createInput.Name, createInput.Date, createInput.Type);

            meal.AsUser(u);

            if (createInput.Ingredients.Any())
            {
                try
                {
                    var ingredients = _ingredientRepository.GetAll().Where(i => createInput.Ingredients.Contains(i.Name))
                        .ToList();
                    meal.SetIngredients(ingredients);
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.ToString());
                }
            }

            return meal;
        }

        /// <inheritdoc />
        protected override void MapToEntity(MealDto updateInput, Meal entity)
        {
            base.MapToEntity(updateInput, entity);

            // Updating changed properties of the retrieved task entity.
            if (!string.IsNullOrEmpty(updateInput.Name))
            {
                entity.SetName(updateInput.Name);
            }
            entity.SetDate(updateInput.Date);

            if (updateInput.Ingredients.Any())
            {
                var ingredients = _ingredientRepository.GetAll(); //.Where(i => updateInput.Ingredients.Contains(i.Name)).ToList();

                entity.SetIngredients(
                    ObjectMapper.Map<List<Ingredient>>(ingredients)
                );
            }
        }

        protected void MapToEntity(UpdateMealDto updateInput, Meal entity)
        {
            // Updating changed properties of the retrieved task entity.
            if (!string.IsNullOrEmpty(updateInput.Name))
            {
                entity.SetName(updateInput.Name);
            }
            entity.SetDate(updateInput.Date);

            entity.SetType(updateInput.Type);

            if (updateInput.Ingredients.Any())
            {
                var ingredients = _ingredientRepository.GetAll().Where(i => updateInput.Ingredients.Contains(i.Name)).ToList();

                entity.SetIngredients(
                    ObjectMapper.Map<List<Ingredient>>(ingredients)
                );
            }
        }

        /// <inheritdoc />
        protected override MealDto MapToEntityDto(Meal entity)
        {
            return base.MapToEntityDto(entity);
        }

        #endregion

#if WRONG
        [DisableValidation]
#endif
        public async void ValidateObject(CreateMealDto input)
        {
            CheckCreatePermission();
            //we can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a meal for input: " + input);

#if WRONG
            // Creating a new Meal entity with given input's properties
            MapToEntity(input, null);
#else
            var user = await GetCurrentUserAsync();
            // Creating a new Meal entity with given input's properties
            var meal = MapToEntity(input, user);
#endif
        }

        public async void SaveValidatedObject(CreateMealDto input)
        {
            CheckCreatePermission();
            //we can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a meal for input: " + input);

#if WRONG
            throw new NotImplementedException();
#else
            var user = await GetCurrentUserAsync();

            // Creating a new Meal entity with given input's properties
            var meal = MapToEntity(input, user);

            // Saving entity with standard insert method of repositories
            var result = await Repository.InsertAsync(meal);
#endif
        }
        public async Task<int> CheckIdSavedValidatedObject(CreateMealDto input)
        {
            CheckCreatePermission();
            //we can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a meal for input: " + input);

            var user = await GetCurrentUserAsync();

            // Creating a new Meal entity with given input's properties
            var meal = MapToEntity(input, user);

            // Saving entity with standard insert method of repositories
            var result = await Repository.InsertAndGetIdAsync(meal);

#if WRONG
            result--; // get previous
#endif
            return result;
        }
    }
}
