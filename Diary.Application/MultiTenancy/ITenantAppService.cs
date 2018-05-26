using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Diary.MultiTenancy.Dto;

namespace Diary.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
