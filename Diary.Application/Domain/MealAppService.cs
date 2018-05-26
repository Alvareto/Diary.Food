using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using AutoMapper;
using Diary.Authorization.Users;
using Diary.Domain.Models;

namespace Diary.Domain
{

    public class DiaryMapProfile : Profile
    {
        public DiaryMapProfile()
        {
            CreateMap<IngredientDto, Ingredient>()
                .ForMember(x => x.Meals, opt => opt.Ignore())
                .ForMember(x => x.NutritionFacts, opt => opt.MapFrom(i => i.NutritionFacts))
                .ReverseMap();
            CreateMap<MealDto, Meal>()
                .ForMember(x => x.Ingredients, opt => opt.MapFrom(i => i.Ingredients))
                .ReverseMap();

            CreateMap<Meal, CreateMealDto>()
                .ForMember(x => x.Ingredients, opt => opt.MapFrom(i => i.Ingredients.Select(m => m.Name)));

        }
    }
    [AutoMap(typeof(Meal))]
    public class MealDto : FullAuditedEntityDto, IEntityDto
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public MealType Type { get; set; }

        public IngredientDto[] Ingredients { get; set; }
    }

    [AutoMap(typeof(Meal))]
    public class CreateMealDto
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public MealType Type { get; set; }

        public string[] Ingredients { get; set; }
    }

    [AutoMap(typeof(Meal))]
    public class UpdateMealDto : FullAuditedEntityDto
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public MealType Type { get; set; }

        public string[] Ingredients { get; set; }
    }


    [AutoMap(typeof(Ingredient))]
    public class IngredientDto : FullAuditedEntityDto, IEntityDto
    {
        public string Name { get; set; }
        public IngredientType Type { get; set; }
        public NutritionFactDto[] NutritionFacts { get; set; }
    }

    [AutoMap(typeof(NutritionFact))]
    public class NutritionFactDto : EntityDto
    {
        public Nutrient Nutrient { get; set; }
        public int Value { get; set; }

        //[NotMapped]
        //public string Units
        //{
        //    get { return Nutrient == Nutrient.Calories ? NutritionFact.DefaultCaloriesUnit : NutritionFact.DefaultUnit; }
        //    set { }
        //}
    }

    public interface INutritionFactAppService : IDiaryAppService<NutritionFactDto>
    {
        Task<ListResultDto<NutritionFactDto>> GetAllDefault();
    }

    public class NutritionFactAppService : DiaryAppServiceBase<NutritionFact, NutritionFactDto>,
        INutritionFactAppService // ICrudAppService<Meal, MealDto>
    {
        public NutritionFactAppService(IRepository<NutritionFact> repository) : base(repository)
        {
        }

        public Task<ListResultDto<NutritionFactDto>> GetAllDefault()
        {
            CheckGetAllPermission();
            var facts = Ingredient.GetDefaultNutritionFacts();

            return Task.FromResult(new ListResultDto<NutritionFactDto>(MapToEntityDtoList(facts)));
        }
    }

    public interface IIngredientAppService : IDiaryAppService<IngredientDto>
    {

    }

    public class IngredientAppService : DiaryAppServiceBase<Ingredient, IngredientDto>,
            IIngredientAppService // ICrudAppService<Meal, MealDto>
    {
        public IngredientAppService(IRepository<Ingredient> repository)
            : base(repository)
        {
        }

        public override async Task<IngredientDto> Get(EntityDto<int> input)
        {
            CheckGetPermission();

            var ingredient = await Repository.GetAllIncluding(m => m.NutritionFacts).SingleOrDefaultAsync(m => m.Id == input.Id);

            if (ingredient == null)
            {
                throw new UserFriendlyException(404, "Meal not found with ID: " + input.Id);
            }

            return MapToEntityDto(ingredient);
        }

        public override async Task<PagedResultDto<IngredientDto>> GetAll(PagedAndSortedResultRequestDto input)
        {
            CheckGetAllPermission();

            var ingredients = await Repository.GetAllIncluding(m => m.NutritionFacts).ToListAsync();
            //var meals = Repository.GetAll().Include(m => m.Ingredients.Select(i => i.NutritionFacts));

            return new PagedResultDto<IngredientDto>(input.MaxResultCount, MapToEntityDtoList(ingredients)); //ObjectMapper.Map<List<MealDto>>(meals)
        }
    }



    public interface ICountAppService : IApplicationService
    {
        IHasTotalCount GetCountMeals();
        IHasTotalCount GetCountMeals(DateTime dateFrom);
        IHasTotalCount GetCountMealType(MealType type);
        IHasTotalCount GetCountIngredients();
        IHasTotalCount GetCountIngredientType(IngredientType type);
        IHasTotalCount GetCountNutritionFacts();
        IHasTotalCount GetCountUsers();
    }

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

        public IHasTotalCount GetCountNutritionFacts()
        {
            return new CountDto { TotalCount = _factRepository.Count() };
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

    public interface IMealAppService : IDiaryAppService<MealDto>
    {
        Task<MealDto> CreateWithNames(CreateMealDto input);
        Task<MealDto> UpdateWithNames(UpdateMealDto input);
    }

    public class MealAppService : DiaryAppServiceBase<Meal, MealDto>, IMealAppService// ICrudAppService<Meal, MealDto>
    {
        protected readonly IRepository<Ingredient> _ingredientRepository;

        public MealAppService(IRepository<Meal> repository, IRepository<Ingredient> ingredientRepository)
            : base(repository)
        {
            _ingredientRepository = ingredientRepository;
        }



        //[UnitOfWork(IsDisabled = true)]
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

        //[UnitOfWork(IsDisabled = true)]
        public override async Task<PagedResultDto<MealDto>> GetAll(PagedAndSortedResultRequestDto input)
        {
            CheckGetAllPermission();

            var meals = await Repository.GetAllIncluding(m => m.Ingredients.Select(i => i.NutritionFacts)).ToListAsync();
            //var meals = Repository.GetAll().Include(m => m.Ingredients.Select(i => i.NutritionFacts));

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

            //var meal = Meal.Create(input.Name, input.Date);

            //if (input.Ingredients.Any())
            //{
            //    try
            //    {
            //        var ingredients = _ingredientRepository.GetAll().Where(i => input.Ingredients.Contains(i.Name))
            //            .ToList();
            //        meal.SetIngredients(ingredients);
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }

            //}

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

            // Saving entity with standard insert method of repositories
            //var result = await Repository.InsertAsync(meal);
            //var map = MapToEntityDto(result);
            //return map;
            return MapToEntityDto(meal);
        }

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var meal = await Repository.GetAsync(input.Id);
            meal.RemoveIngredients();

            await Repository.DeleteAsync(meal);
        }

        /// <inheritdoc />
        protected override Meal MapToEntity(MealDto createInput)
        {
            var meal = base.MapToEntity(createInput);

            // Creating a new Meal entity with given input's properties
            //var meal = Meal.Create(createInput.Name, createInput.Date);

            //if (createInput.Ingredients.Any())
            //{
            //    meal.AddIngredients(
            //        ObjectMapper.Map<List<Ingredient>>(createInput.Ingredients)
            //    );
            //}

            return meal;
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

        //public MealDto Get(EntityDto<int> input)
        //{
        //    
        //}

        //public PagedResultDto<MealDto> GetAll(PagedAndSortedResultRequestDto input)
        //{
        //    var meals = _mealRepository.GetAll().Include(m => m.Ingredients.Select(i => i.NutritionFacts));

        //    return new PagedResultDto<MealDto>(input.MaxResultCount, Mapper.Map<List<MealDto>>(meals));
        //}

        //public MealDto Create(MealDto input)
        //{
        //    // we can use Logger, it's defined in ApplicationService class.
        //    Logger.Info("Creating a meal for input: " + input);

        //    // Creating a new Meal entity with given input's properties
        //    var meal = Meal.Create(input.Name, input.Date);

        //    if (input.Ingredients.Any())
        //    {
        //        //_manager.SetMealIngredients(meal, input.Ingredients);

        //        meal.AddIngredients
        //        (
        //            Mapper.Map<List<Ingredient>>(input.Ingredients)
        //        ); // TODO: dto
        //    }

        //    // Saving entity with standard inset method of repositories
        //    _mealRepository.Insert(meal);

        //    return Mapper.Map<MealDto>(meal);
        //}

        //public MealDto Update(MealDto input)
        //{
        //    // we can use Logger, it's defined in ApplicationService base class.
        //    Logger.Info("Updating a task for input: " + input);

        //    // retrieving a meal entity with given id using standard Get method of repositories.
        //    var meal = _mealRepository.Get(input.Id);

        //    // Updating changed properties of the retrieved task entity.
        //    if (!string.IsNullOrEmpty(input.Name))
        //    {
        //        meal.SetName(input.Name);
        //    }
        //    meal.SetDate(input.Date);
        //    //if (input.Date.HasValue)
        //    //{
        //    //    meal.SetDate(input.Date.Value);
        //    //}
        //    return Mapper.Map<MealDto>(meal);
        //}

        //public void Delete(EntityDto<int> input)
        //{
        //    var meal = _mealRepository.Get(input.Id);
        //    meal.RemoveIngredients();

        //    _mealRepository.Delete(meal);
        //}

    }
}
