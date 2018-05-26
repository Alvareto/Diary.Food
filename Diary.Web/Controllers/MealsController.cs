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
using Abp.Domain.Uow;
using Abp.Timing;
using Diary.Domain;
using Diary.Domain.Models;
using Diary.EntityFramework;
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

        // GET: Meals/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    //var role = await _roleAppService.Get(new EntityDto(roleId));
        //    //var permissions = (await _roleAppService.GetAllPermissions()).Items;
        //    //var model = new EditRoleModalViewModel
        //    //{
        //    //    Role = role,
        //    //    Permissions = permissions
        //    //};
        //    //return View("_EditRoleModal", model);

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var meal = await _mealService.Get(new EntityDto(id.Value));

        //    if (meal == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(meal);
        //}

        //// GET: Meals/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CreatorUserId = new SelectList(db.Users, "Id", "Name");
        //    return View();
        //}

        //// POST: Meals/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Date,CreatorUserId")] Meal meal)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ////_mealAppService.Create()
        //        //db.Meals.Add(meal);
        //        //db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    //ViewBag.CreatorUserId = new SelectList(db.Users, "Id", "AuthenticationSource", meal.CreatorUserId);
        //    return View(meal);
        //}

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

        //// POST: Meals/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,Date,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId")] Meal meal)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //db.Entry(meal).State = EntityState.Modified;
        //        //db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.CreatorUserId = new SelectList(db.Users, "Id", "AuthenticationSource", meal.CreatorUserId);
        //    return View(meal);
        //}

        //// GET: Meals/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MealDto meal = _mealAppService.Get(new EntityDto(id.Value));// db.Meals.Find(id);
        //    if (meal == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View();
        //}

        //// POST: Meals/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    //Meal meal = db.Meals.Find(id);
        //    //db.Meals.Remove(meal);
        //    //db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{

            //}
            base.Dispose(disposing);
        }
    }
}
