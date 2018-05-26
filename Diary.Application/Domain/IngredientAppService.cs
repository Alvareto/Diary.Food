using System.Data.Entity;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Diary.Domain.Dto;
using Diary.Domain.Models;

namespace Diary.Domain
{
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
}