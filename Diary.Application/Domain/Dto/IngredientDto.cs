using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Diary.Domain.Models;

namespace Diary.Domain.Dto
{
    [AutoMap(typeof(Ingredient))]
    public class IngredientDto : FullAuditedEntityDto, IEntityDto, IApprovalProcessDto
    {
        public string Name { get; set; }
        public IngredientType Type { get; set; }
        public NutritionFactDto[] NutritionFacts { get; set; }
        public ApprovalStatus Status { get; set; }
    }
}