using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Abp.UI;
using Diary.Authorization.Users;
using Diary.Domain.Dto;
using Diary.Domain.Events;
using Diary.Domain.Models;

namespace Diary.Domain
{
    public class IngredientAppService : DiaryAppServiceBase<Ingredient, IngredientDto>,
        IIngredientAppService // ICrudAppService<Meal, MealDto>
    {
        public IEventBus EventBus { get; set; }

        public IngredientAppService(IRepository<Ingredient> repository)
            : base(repository)
        {
            EventBus = NullEventBus.Instance;
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

        public async Task<PagedResultDto<IngredientDto>> GetAllPending(PagedAndSortedResultRequestDto input)
        {
            CheckGetAllPermission();

            var ingredients = await Repository.GetAllIncluding(m => m.NutritionFacts).Where(i => i.Status == ApprovalStatus.Pending).ToListAsync();
            //var meals = Repository.GetAll().Include(m => m.Ingredients.Select(i => i.NutritionFacts));

            return new PagedResultDto<IngredientDto>(input.MaxResultCount, MapToEntityDtoList(ingredients)); //ObjectMapper.Map<List<MealDto>>(meals)
        }

        public async Task Approve(EntityDto<int> input)
        {
            CheckUpdatePermission();
            //we can use Logger, it's defined in ApplicationService class.
            Logger.Info("Approving an ingredient for id: " + input.Id);

            // retrieving a ingredient entity with given id using standard Get method of repositories.
            var ingredient = await Repository.GetAsync(input.Id);
            var user = await GetCurrentUserAsync();

            ingredient.Status = ApprovalStatus.Approved;
            ingredient.ApproverUser = user;

            await Repository.UpdateAsync(ingredient);

            await EventBus.TriggerAsync(new EntityApprovedEventData<Ingredient> { Entity = ingredient });
        }

        public async Task Reject(EntityDto<int> input)
        {
            CheckUpdatePermission();

            Logger.Info("Rejecting an ingredient for id: " + input.Id);

            var ingredient = await Repository.GetAsync(input.Id);
            var user = await GetCurrentUserAsync();

            ingredient.Status = ApprovalStatus.Rejected;
            ingredient.ApproverUser = user;

            await Repository.UpdateAsync(ingredient);

            await EventBus.TriggerAsync(new EntityRejectedEventData<Ingredient> { Entity = ingredient });
        }
    }
}