using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Diary.Domain.Models;

namespace Diary.Domain.Dto
{
    [AutoMap(typeof(Meal))]
    public class CreateMealDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public MealType Type { get; set; }

        public string[] Ingredients { get; set; }
    }
}