using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Diary.Domain.Dto;

namespace Diary.Domain
{
    public interface INutritionFactAppService : IDiaryAppService<NutritionFactDto>
    {
        Task<ListResultDto<NutritionFactDto>> GetAllDefault();
    }
}