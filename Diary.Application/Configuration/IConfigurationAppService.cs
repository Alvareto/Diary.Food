using System.Threading.Tasks;
using Abp.Application.Services;
using Diary.Configuration.Dto;

namespace Diary.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}