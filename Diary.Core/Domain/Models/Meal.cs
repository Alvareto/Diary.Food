using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Abp.UI;
using Diary.Authorization.Users;

namespace Diary.Domain.Models
{

    public class Ingredient : FullAuditedEntity
    {
        public const int MaxNameLength = 256;

        [Required]
        [MaxLength(MaxNameLength)]
        public virtual string Name { get; protected set; }

        [Column("IngredientType")]
        public virtual IngredientType Type { get; protected set; }

        public virtual ICollection<NutritionFact> NutritionFacts { get; protected set; }
        public virtual ICollection<Meal> Meals { get; protected set; }

        protected Ingredient()
        {
            //this.NutritionFacts = new HashSet<NutritionFact>();
            //this.Meals = new HashSet<Meal>();
        }

        public static Ingredient Create(string name, IngredientType type = IngredientType.Other, List<NutritionFact> facts = null)
        {
            var i = new Ingredient
            {
                Name = name,
                NutritionFacts = facts ?? GetDefaultNutritionFacts()
            };

            return i;
        }

        public void AddOrChangeDeclaration(Nutrient nutrient, int value)
        {
            var fact = NutritionFacts.FirstOrDefault(f => f.Nutrient == nutrient);
            if (fact == null)
            {
                fact = NutritionFact.Create(nutrient, value);
                NutritionFacts.Add(fact);
            }
            else
            {
                foreach (var f in NutritionFacts.Where(i => i.Nutrient == nutrient))
                {
                    f.ChangeValue(value);
                }
            }
        }

        public static List<NutritionFact> GetDefaultNutritionFacts()
        {
            List<NutritionFact> t = new List<NutritionFact>();

            foreach (Nutrient n in Enum.GetValues(typeof(Nutrient)))
            {
                t.Add(NutritionFact.Create(n));
            }

            return t;
        }
    }

    public class NutritionFact : FullAuditedEntity
    {
        public const int MinValue = 0;
        public const int DefaultValue = 0;
        public const string DefaultUnit = "grams";
        public const string DefaultCaloriesUnit = "kcal";

        [Required]
        public virtual Nutrient Nutrient { get; protected set; }

        [Required]
        [DefaultValue(DefaultValue)]
        [Range(MinValue, int.MaxValue)]
        public virtual int Value { get; protected set; }

        

        protected NutritionFact()
        {

        }

        public static NutritionFact Create(Nutrient nutrient, int value = 0)
        {
            var fact = new NutritionFact
            {
                Nutrient = nutrient,
                Value = value
            };

            return fact;
        }

        public void ChangeValue(int value)
        {
            Value = value;
        }


    }

    public enum Nutrient
    {
        Calories,
        Fat,
        SaturatedFat,
        PolyunsaturatedFat,
        MonounsaturatedFat,
        TransFat,
        Cholesterol,
        Sodium,
        Potassium,
        Carbohydrates,
        Fiber,
        Sugar,
        Protein,
        VitaminA,
        VitaminC,
        Calcium,
        Iron
    }

    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack,
        Other
    }

    public enum IngredientType
    {
        Meat,
        Vegetable,
        Fruit,
        Drink,
        Other
    }

    public class Meal : FullAuditedEntity
    {
        public const int MaxNameLength = 256;

        [Required]
        [MaxLength(MaxNameLength)]
        public virtual string Name { get; protected set; }

        [Required]
        [Column("MealDate")]
        public virtual DateTime Date { get; protected set; }

        [Column("MealType")]
        public virtual MealType Type { get; protected set; }

        [Required]
        [ForeignKey("CreatorUserId")]
        public virtual User User { get; protected set; }

        public virtual ICollection<Ingredient> Ingredients { get; protected set; }

        protected Meal()
        {
            //this.Ingredients = new HashSet<Ingredient>();
        }

        public static Meal Create(string name)
        {
            return Meal.Create(name, Clock.Now);
        }

        public static Meal Create(string name, DateTime date, MealType type = MealType.Other)
        {
            var meal = new Meal();

            meal.SetName(name);

            meal.SetDate(date);

            meal.SetType(type);

            meal.Ingredients = new List<Ingredient>();

            return meal;
        }

        public void AsUser(User u)
        {
            this.User = u;
        }

        public void AddIngredient(Ingredient ingredient)
        {
            this.Ingredients.Add(ingredient);
        }

        public void AddIngredients(List<Ingredient> ingredients)
        {
            foreach (var i in ingredients)
            {
                AddIngredient(i);
            }
        }

        public void RemoveIngredient(Ingredient ingredient)
        {
            this.Ingredients.Remove(ingredient);
        }

        public void RemoveIngredients(List<Ingredient> ingredients)
        {
            foreach (var i in ingredients)
            {
                RemoveIngredient(i);
            }
        }

        public void RemoveIngredients()
        {
            this.Ingredients.Clear(); //= new List<Ingredient>();
        }

        public void SetDate(DateTime date)
        {
            if (date.Date > Clock.Now.Date)
            {
                throw new UserFriendlyException("Can not set meal's date in the future!");
            }

            Date = date;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public void SetType(MealType type)
        {
            this.Type = type;
        }

        public void SetIngredients(IList<Ingredient> ingredients)
        {
            var webIngredients = ingredients;
            var dbIngredients = this.Ingredients.ToList();
            var ingredientsToDelete = dbIngredients.Where(db => !webIngredients.Contains(db)).ToList(); // it is in db, but not in web => DELETE
            var ingredientsToAdd = webIngredients.Where(web => !dbIngredients.Contains(web)).ToList(); // if it's already in db, skip

            // Delete removed ingredients
            RemoveIngredients(ingredientsToDelete);

            // Add ingredients that meal doesn't already have
            AddIngredients(ingredientsToAdd);
        }
    }

}
