using System.Linq;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using Abp.Timing;
using Diary.Domain;
using Diary.Domain.Dto;
using Diary.Domain.Models;
using Shouldly;
using Xunit;

namespace Diary.Tests
{
    public class MealAppService_Tests : DiaryTestBase
    {
        private readonly IMealAppService _service;

        public MealAppService_Tests()
        {
            // creating the class which is tested (SUT - Software Under Test)
            _service = LocalIocManager.Resolve<IMealAppService>();
        }

        /// <summary>
        /// AAA pattern = Arrange, Act, Assert
        /// </summary>
        [Fact]
        public void Should_Create_New_Meal()
        {
            //Arrange: Prepare for test
            var initialMealCount = UsingDbContext(context => context.Meals.Count());
            var user = UsingDbContext(context => context.Users.First());

            //Act: Run SUT
            _service.CreateWithNames(
                new CreateMealDto
                {
                    Name = "TestMeal",
                    Date = Clock.Now,
                    Type = MealType.Breakfast,
                    Ingredients = new string[] { }
                });

            //Assert: Check results
            UsingDbContext(context =>
            {
                context.Meals.Count().ShouldBe(initialMealCount + 1);
                var meal = context.Meals.FirstOrDefault(m => m.Name == "TestMeal");
                meal.ShouldNotBe(null);
                meal.Type.ShouldBe(MealType.Breakfast);
                
            });
        }

        [Fact]
        public async Task Should_Not_Create_Meal_Without_Required_Properties()
        {
            await Assert.ThrowsAsync<AbpValidationException>(() => _service.CreateWithNames(new CreateMealDto()));
        }

        //public void SH
    }
}