using System.Text.Json;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Domain.Entities;

namespace Menufy.Application.Features.MenuTemplates.Helpers;

public static class MenuTemplateMapper
{
    public static MenuTemplateDto ToDto(MenuTemplate entity)
    {
        return new MenuTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            RestaurantId = entity.RestaurantId,
            IsPublished = entity.IsPublished,
            Tags = DeserializeList(entity.Tags),
            Theme = Deserialize<MenuTemplateThemeDto>(entity.Theme),
            Structure = Deserialize<MenuTemplateStructureDto>(entity.Structure) ?? new MenuTemplateStructureDto(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static void UpdateEntity(MenuTemplate entity, UpsertMenuTemplateDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.RestaurantId = dto.RestaurantId;
        entity.IsPublished = dto.IsPublished;
        entity.Tags = Serialize(dto.Tags);
        entity.Theme = Serialize(dto.Theme);
        entity.Structure = Serialize(dto.Structure) ?? JsonSerializer.Serialize(new MenuTemplateStructureDto(), MenuTemplateSerializationOptions.Options);
    }

    public static MenuTemplate CreateEntity(UpsertMenuTemplateDto dto)
    {
        return new MenuTemplate
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            RestaurantId = dto.RestaurantId,
            IsPublished = dto.IsPublished,
            Tags = Serialize(dto.Tags),
            Theme = Serialize(dto.Theme),
            Structure = Serialize(dto.Structure) ?? JsonSerializer.Serialize(new MenuTemplateStructureDto(), MenuTemplateSerializationOptions.Options)
        };
    }

    private static string? Serialize<T>(T value)
    {
        if (value == null)
            return null;

        return JsonSerializer.Serialize(value, MenuTemplateSerializationOptions.Options);
    }

    private static T? Deserialize<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, MenuTemplateSerializationOptions.Options);
    }

    private static List<string> DeserializeList(string? json)
    {
        return Deserialize<List<string>>(json) ?? new List<string>();
    }
}

