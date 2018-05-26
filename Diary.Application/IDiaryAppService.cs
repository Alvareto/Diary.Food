using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Diary
{
    public interface IDiaryAppService : IApplicationService
    {

    }

    public interface IDiaryAppService<TEntityDto>
        : IAsyncCrudAppService<TEntityDto> where TEntityDto : IEntityDto
    {

    }
}