using System.Linq;
using AutoMapper;
using Diary.Domain.Models;

namespace Diary.Domain.Dto
{
    public class DiaryMapProfile : Profile
    {
        public DiaryMapProfile()
        {
            CreateMap<IngredientDto, Ingredient>()
                .ForMember(x => x.Meals, opt => opt.Ignore())
                .ForMember(x => x.NutritionFacts, opt => opt.MapFrom(i => i.NutritionFacts))
                .ReverseMap();
            CreateMap<MealDto, Meal>()
                .ForMember(x => x.Ingredients, opt => opt.MapFrom(i => i.Ingredients))
                .ReverseMap();

            CreateMap<Meal, CreateMealDto>()
                .ForMember(x => x.Ingredients, opt => opt.MapFrom(i => i.Ingredients.Select(m => m.Name)));

        }
    }
}