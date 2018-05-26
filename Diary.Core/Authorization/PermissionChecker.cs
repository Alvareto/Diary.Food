using Abp.Authorization;
using Diary.Authorization.Roles;
using Diary.Authorization.Users;

namespace Diary.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
