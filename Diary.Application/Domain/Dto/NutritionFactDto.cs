using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Diary.Domain.Models;

namespace Diary.Domain.Dto
{
    [AutoMap(typeof(NutritionFact))]
    public class NutritionFactDto : EntityDto
    {
        public Nutrient Nutrient { get; set; }
        public int Value { get; set; }
    }
}