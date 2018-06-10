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
        /// Instancirati objekt poslovnog sloja te testirati neko validacijsko pravilo
        /// </summary>
        [Fact]
        public void Should_ValidateObject()
        {
            //Arrange: Prepare for test
            var dto = new CreateMealDto
            {
                Date = Clock.Now.AddMonths(5), // can't have future date
            };

            //Act: Run SUT
            var ex = Record.Exception(() => _service.ValidateObject(dto));

            //Assert: Check results
            Assert.IsType<AbpValidationException>(ex);
        }

        /// <summary>
        /// AAA pattern = Arrange, Act, Assert
        /// Testirati metodu koja æe provjereni objekt zapisati u bazu podataka
        /// </summary>
        [Fact]
        public void Should_SaveValidatedObject()
        {
            //Arrange: Prepare for test
            var dto = new CreateMealDto
            {
                Name = "TestMeal",
                Date = Clock.Now,
                Type = MealType.Breakfast,
                Ingredients = new string[] { }
            };

            //Act: Run SUT
            var ex = Record.Exception(() => _service.SaveValidatedObject(dto));

            //Assert: Check results
            Assert.Null(ex);
        }

        /// <summary>
        /// AAA pattern = Arrange, Act, Assert
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