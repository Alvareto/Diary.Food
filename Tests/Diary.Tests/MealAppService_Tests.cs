using System.Linq;
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

        [Fact]
        public void Should_Create_New_Meals()
        {
            // Prepare for test
            var initialMealCount = UsingDbContext(context => context.Meals.Count());
            var user = UsingDbContext(context => context.Users.First());

            // Run SUT
            _service.CreateWithNames(
                new CreateMealDto
                {
                    Name = "TestMeal",
                    Date = Clock.Now,
                    Type = MealType.Breakfast,
                    Ingredients = new string[] { }
                });

            // Check results
            UsingDbContext(context =>
            {
                context.Meals.Count().ShouldBe(initialMealCount + 1);
                var meal = context.Meals.FirstOrDefault(m => m.Name == "TestMeal");
                meal.ShouldNotBe(null);
                meal.Type.ShouldBe(MealType.Breakfast);
                
            });
        }
    }
}