using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Diary.Domain.Models;

namespace Diary.Domain.Dto
{
    [AutoMap(typeof(Meal))]
    public class MealDto : FullAuditedEntityDto, IEntityDto
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public MealType Type { get; set; }

        public IngredientDto[] Ingredients { get; set; }
    }
}