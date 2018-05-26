using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace Diary.Domain.Models
{
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
}