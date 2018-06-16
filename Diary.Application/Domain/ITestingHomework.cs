using System.Threading.Tasks;
using Diary.Domain.Dto;

namespace Diary.Domain
{
    public interface ITestingHomework
    {
        void ValidateObject(CreateMealDto input);

        void SaveValidatedObject(CreateMealDto input);

        Task<int> CheckIdSavedValidatedObject(CreateMealDto input);
    }
}