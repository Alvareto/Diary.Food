using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Diary.Domain.Dto;

namespace Diary.Domain
{
    public interface IIngredientAppService : IDiaryAppService<IngredientDto>
    {
        Task Approve(EntityDto<int> input);
        Task Reject(EntityDto<int> input);
    }
}