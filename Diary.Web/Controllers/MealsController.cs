using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Abp.Domain.Uow;
using Abp.Timing;
using Diary.Domain;
using Diary.Web.Models.Meals;

namespace Diary.Web.Controllers
{
    public class MealsController : DiaryControllerBase
    {
        //private DiaryDbContext db;
        private readonly IMealAppService _mealService;

        private readonly IIngredientAppService _ingredientService;

        public MealsController(IMealAppService mealService, IIngredientAppService ingredientService)
        {
            _mealService = mealService;
            _ingredientService = ingredientService;
            //db = new DiaryDbContext();
        }

        // GET: Meals
        [UnitOfWork(IsDisabled = true)]
        public async Task<ActionResult> Index(DateTime? t)
        {
            var date = t ?? Clock.Now;

            var meals = (await _mealService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = int.MaxValue })).Items;
            var dayMeals = meals.Where(m => m.Date.Date == date.Date).ToList();
            var ingredients = (await _ingredientService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = int.MaxValue })).Items;
            //var users = (await _userAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue })).Items; //Paging not implemented yet
            //var roles = (await _userAppService.GetRoles()).Items;
            var vm = new MealListViewModel()
            {
                Meals = dayMeals,
                Day = date,
                Ingredients = ingredients
            };

            //var meals = db.Meals.Include(m => m.User);
            return View(vm);
        }

        // GET: Meals/Edit/5
        public async Task<ActionResult> EditMealModal(int mealId)
        {
            var meal = await _mealService.Get(new EntityDto(mealId));// db.Meals.Find(id);
            var ingredients = (await _ingredientService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = int.MaxValue })).Items;
            var vm = new EditMealModalViewModel
            {
                Meal = meal,
                Ingredients = ingredients
            };
            return View("_EditMealModal", vm);
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{

            //}
            base.Dispose(disposing);
        }
    }
}
