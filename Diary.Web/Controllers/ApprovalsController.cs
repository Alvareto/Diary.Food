using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Diary.Domain;
using Diary.Domain.Models;
using Diary.Web.Models.Ingredients;

namespace Diary.Web.Controllers
{
    public class ApprovalsController : DiaryControllerBase
    {
        private readonly IIngredientAppService _ingredientService;
        private readonly INutritionFactAppService _nutritionFactService;

        public ApprovalsController(IIngredientAppService ingredientService, INutritionFactAppService nutritionFactService)
        {
            _ingredientService = ingredientService;
            _nutritionFactService = nutritionFactService;
        }

        // GET: Ingredients
        public async Task<ActionResult> Index()
        {
            var ingredients = (await _ingredientService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = int.MaxValue })).Items
                .Where(i => i.Status == ApprovalStatus.Pending).ToList(); // TODO: move to appservice
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

        public async Task<ActionResult> Accept(int ingredientId)
        {
            var ingredient = await _ingredientService.Get(new EntityDto(ingredientId));

            var wf = new ApprovalProcess(_ingredientService, ingredient);
            wf.Approve(ingredient);

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
        public async Task<ActionResult> Reject(int ingredientId)
        {
            var ingredient = await _ingredientService.Get(new EntityDto(ingredientId));

            var wf = new ApprovalProcess(_ingredientService, ingredient);
            wf.Reject(ingredient);

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
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
