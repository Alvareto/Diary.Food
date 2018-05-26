using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Diary.Roles.Dto;
using Diary.Users.Dto;

namespace Diary.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
    }
}