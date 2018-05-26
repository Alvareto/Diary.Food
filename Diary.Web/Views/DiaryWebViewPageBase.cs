using Abp.Web.Mvc.Views;

namespace Diary.Web.Views
{
    public abstract class DiaryWebViewPageBase : DiaryWebViewPageBase<dynamic>
    {

    }

    public abstract class DiaryWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected DiaryWebViewPageBase()
        {
            LocalizationSourceName = DiaryConsts.LocalizationSourceName;
        }
    }
}