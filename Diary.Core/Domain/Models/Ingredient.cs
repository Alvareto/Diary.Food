using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.Domain.Entities.Auditing;

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
}