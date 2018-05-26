using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Diary.Domain.Dto;
using Diary.Domain.Models;

namespace Diary.Domain
{
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
}