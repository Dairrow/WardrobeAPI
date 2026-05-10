using AutoMapper;
using Wardrobe.API.DTOs.Brands;
using Wardrobe.API.DTOs.Categories;
using Wardrobe.API.DTOs.Roles;
using Wardrobe.API.DTOs.Users;
using Wardrobe.Data.Entities;
using Wardrobe.API.DTOs.ClothingItems;
using Wardrobe.API.DTOs.Outfits;
using Wardrobe.API.DTOs.OutfitItems;

namespace Wardrobe.API.Profiles;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<Category, CategoryDto>();

        CreateMap<CreateCategoryDto, Category>();


        CreateMap<Brand, BrandDto>();

        CreateMap<CreateBrandDto, Brand>();

        CreateMap<
            CreateClothingItemDto,
            ClothingItem>()
            .ForMember(
                x => x.ImagePath,
                opt => opt.Ignore());


        CreateMap<
            UpdateClothingItemDto,
            ClothingItem>()
            .ForMember(
                x => x.ImagePath,
                opt => opt.Ignore());

        CreateMap<ClothingItem, ClothingItemDto>()
            .ForMember(
                x => x.CategoryName,
                opt => opt.MapFrom(
                    src => src.Category.Name))
            .ForMember(
                x => x.BrandName,
                opt => opt.MapFrom(
                    src => src.Brand.Name));


        CreateMap<CreateOutfitDto, Outfit>();

        CreateMap<OutfitItem, OutfitItemDto>();

        CreateMap<Outfit, OutfitDto>();

        CreateMap<Role, RoleDto>();


        CreateMap<CreateUserDto, User>()
            .ForMember(
                x => x.PasswordHash,
                opt => opt.Ignore());


        CreateMap<User, UserDto>()
            .ForMember(
                x => x.RoleName,
                opt => opt.MapFrom(
                    src => src.Role.Name));

        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<UpdateBrandDto, Brand>();

        CreateMap<UpdateUserDto, User>();

        CreateMap<UpdateClothingItemDto, ClothingItem>();

        CreateMap<UpdateOutfitDto, Outfit>();
    }
}