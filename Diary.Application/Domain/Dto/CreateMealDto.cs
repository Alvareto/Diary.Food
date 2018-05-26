using System;
using Abp.AutoMapper;
using Diary.Domain.Models;

namespace Diary.Domain.Dto
{
    [AutoMap(typeof(Meal))]
    public class CreateMealDto
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public MealType Type { get; set; }

        public string[] Ingredients { get; set; }
    }
}