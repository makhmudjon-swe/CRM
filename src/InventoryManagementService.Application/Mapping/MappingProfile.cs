using AutoMapper;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Domain.Entities;

namespace InventoryManagementService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Inventory, InventoryDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner.DisplayName ?? s.Owner.Email))
            .ForMember(d => d.ItemCount, opt => opt.MapFrom(s => s.Items.Count))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.InventoryTags.Select(it => it.Tag.Name).ToList()));

        CreateMap<CreateInventoryDto, Inventory>();
        CreateMap<UpdateInventoryDto, Inventory>()
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<Item, ItemDto>()
            .ForMember(d => d.LikeCount, opt => opt.MapFrom(s => s.Likes.Count));
        CreateMap<CreateItemDto, Item>();
        CreateMap<UpdateItemDto, Item>()
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<Category, CategoryDto>();
        CreateMap<Tag, TagDto>()
            .ForMember(d => d.UsageCount, opt => opt.MapFrom(s => s.InventoryTags.Count));

        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author.DisplayName ?? s.Author.Email))
            .ForMember(d => d.AuthorAvatarUrl, opt => opt.MapFrom(s => s.Author.AvatarUrl));

        CreateMap<InventoryAccess, InventoryAccessDto>()
            .ForMember(d => d.UserDisplayName, opt => opt.MapFrom(s => s.User.DisplayName))
            .ForMember(d => d.UserEmail, opt => opt.MapFrom(s => s.User.Email));

        CreateMap<ApplicationUser, UserDto>();
    }
}
