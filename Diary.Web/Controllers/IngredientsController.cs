using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Diary.Domain;
using Diary.Web.Models.Ingredients;

namespace Diary.Web.Controllers
{
    public class IngredientsController : DiaryControllerBase
    {
        private readonly IIngredientAppService _ingredientService;
        private readonly INutritionFactAppService _nutritionFactService;

        public IngredientsController(IIngredientAppService ingredientService, INutritionFactAppService nutritionFactService)
        {
            _ingredientService = ingredientService;
            _nutritionFactService = nutritionFactService;
        }

        // GET: Ingredients
        public async Task<ActionResult> Index()
        {
            var ingredients = (await _ingredientService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = int.MaxValue })).Items;
            var facts = (await _nutritionFactService.GetAllDefault()).Items;

            var vm = new IngredientListViewModel()
            {
                Ingredients = ingredients,
                NutritionFacts = facts
            };

            return View(vm);
        }

        public async Task<ActionResult> EditIngredientModal(int ingredientId)
        {
            var ingredient = await _ingredientService.Get(new EntityDto(ingredientId));
            var facts = ingredient.NutritionFacts;

            var vm = new EditIngredientModalViewModel()
            {
                Ingredient = ingredient,
                NutritionFacts = facts
            };

            return View("_EditIngredientModal", vm);
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    db.Dispose();
            //}
            base.Dispose(disposing);
        }
    }
}
