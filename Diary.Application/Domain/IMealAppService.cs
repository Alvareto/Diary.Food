using System.Threading.Tasks;
using Diary.Domain.Dto;

namespace Diary.Domain
{
    public interface IMealAppService : IDiaryAppService<MealDto>
    {
        Task<MealDto> CreateWithNames(CreateMealDto input);
        Task<MealDto> UpdateWithNames(UpdateMealDto input);
    }
}