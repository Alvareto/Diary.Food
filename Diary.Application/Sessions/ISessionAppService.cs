using System.Threading.Tasks;
using Abp.Application.Services;
using Diary.Sessions.Dto;

namespace Diary.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
