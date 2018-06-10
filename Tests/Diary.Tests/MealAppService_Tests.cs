using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
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
        //[Fact]
        //public void Should_Create_New_Meal()
        //{
        //    //Arrange: Prepare for test
        //    var initialMealCount = UsingDbContext(context => context.Meals.Count());

        //    LoginAsHostAdmin();

        //    //Act: Run SUT
        //    _service.CreateWithNames(
        //        new CreateMealDto
        //        {
        //            Name = "TestMeal",
        //            Date = Clock.Now,
        //            Type = MealType.Breakfast,
        //            Ingredients = new string[] { }
        //        });

        //    //Assert: Check results
        //    UsingDbContext(context =>
        //    {
        //        context.Meals.Count().ShouldBe(initialMealCount + 1);
        //        var meal = context.Meals.FirstOrDefault(m => m.Name == "TestMeal");
        //        meal.ShouldNotBe(null);
        //        meal.Type.ShouldBe(MealType.Breakfast);

        //    });
        //}

        //[Fact]
        //public void Should_Pass_Validation_Rule()
        //{
        //    //_service.WRONG_CreateWithNames(
        //    //    new CreateMealDto
        //    //    {
        //    //        Name = "TestMeal",
        //    //        Date = Clock.Now,
        //    //        Type = MealType.Breakfast,
        //    //        Ingredients = new string[] { }
        //    //    });
        //}

        //[Fact]
        //public async Task Should_Not_Create_Meal_Without_Required_Properties()
        //{
        //    await Assert.ThrowsAsync<AbpValidationException>(() => _service.CreateWithNames(new CreateMealDto()));
        //}




        /// <summary>
        /// Instancirati objekt poslovnog sloja te testirati neko validacijsko pravilo
        /// </summary>
        [Fact]
        public void Should_ValidateObject()
        {
            Assert.Throws<AbpValidationException>(() => _service.ValidateObject(
                new CreateMealDto
                {
                    Date = Clock.Now.AddMonths(5), // can't have future date
                }));
        }

        /// <summary>
        /// Testirati metodu koja æe provjereni objekt zapisati u bazu podataka
        /// </summary>
        [Fact]
        public void Should_SaveValidatedObject()
        {
            //Act: Run SUT
            _service.SaveValidatedObject(
                new CreateMealDto
                {
                    Name = "TestMeal",
                    Date = Clock.Now,
                    Type = MealType.Breakfast,
                    Ingredients = new string[] { }
                });
        }

        /// <summary>
        /// Testirati metodu koja èitanjem dokazuje da je u BP zapisan ispravan podatak
        /// </summary>
        [Fact]
        public async Task Should_CheckSavedValidatedObject()
        {
            //Arrange: Prepare for test
            // we can work with repositories instead of DbContext
            var mealRepository = LocalIocManager.Resolve<IRepository<Meal>>();
            var dto = new CreateMealDto
            {
                Name = "TestMeal",
                Date = Clock.Now,
                Type = MealType.Breakfast,
                Ingredients = new string[] { }
            };

            //Act: Run SUT
            // obtain test data
            var mealID = await _service.CheckIdSavedValidatedObject(dto);

            //Assert: Check results
            var meal = mealRepository.Get(mealID);
            meal.ShouldNotBe(null);
            meal.Name.ShouldBe(dto.Name);
        }
    }
}