using System.Collections.Generic;
using Diary.Roles.Dto;
using Diary.Users.Dto;

namespace Diary.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}