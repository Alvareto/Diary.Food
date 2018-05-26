using System.Threading.Tasks;
using Abp.Application.Services;
using Diary.Authorization.Accounts.Dto;

namespace Diary.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
