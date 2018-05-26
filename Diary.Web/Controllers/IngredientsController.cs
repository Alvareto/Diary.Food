using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Diary.Domain;
using Diary.Domain.Models;
using Diary.EntityFramework;
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

        // GET: Ingredients/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ingredient ingredient = db.Ingredients.Find(id);
        //    if (ingredient == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ingredient);
        //}

        //// GET: Ingredients/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Ingredients/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId")] Ingredient ingredient)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Ingredients.Add(ingredient);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(ingredient);
        //}
        // GET: Ingredients/Edit/5
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

        //// POST: Ingredients/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId")] Ingredient ingredient)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ingredient).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(ingredient);
        //}

        //// GET: Ingredients/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ingredient ingredient = db.Ingredients.Find(id);
        //    if (ingredient == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ingredient);
        //}

        //// POST: Ingredients/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Ingredient ingredient = db.Ingredients.Find(id);
        //    db.Ingredients.Remove(ingredient);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
