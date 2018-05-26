//using Abp.Configuration;
using Abp.IdentityFramework;
using Abp.UI;
using Abp.Web.Mvc.Controllers;
//using Diary.Configuration;
using Microsoft.AspNet.Identity;

namespace Diary.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class DiaryControllerBase : AbpController
    {
        //public string UITheme =  SettingManager.GetSettingValue(AppSettingNames.UiTheme);

        protected DiaryControllerBase()
        {
            LocalizationSourceName = DiaryConsts.LocalizationSourceName;
        }

        protected virtual void CheckModelState()
        {
            if (!ModelState.IsValid)
            {
                throw new UserFriendlyException(L("FormIsNotValidMessage"));
            }
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}