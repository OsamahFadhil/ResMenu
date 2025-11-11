using FluentAssertions;
using Menufy.Application.Features.Menus.Commands.CreateCategory;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Domain.Entities;
using Menufy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Menufy.Application.Tests.Menus;

public class CreateCategoryCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"MenufyTests-{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _handler = new CreateCategoryCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenRestaurantDoesNotExist()
    {
        var command = new CreateCategoryCommand(Guid.NewGuid(), new CreateCategoryDto
        {
            Name = "Brunch",
            DisplayOrder = 1
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Restaurant not found");
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenParentDoesNotBelongToRestaurant()
    {
        var targetRestaurant = await SeedRestaurantAsync("Target Restaurant");
        var otherRestaurant = await SeedRestaurantAsync("Other Restaurant");

        var foreignParent = new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = "Foreign Parent",
            DisplayOrder = 0,
            RestaurantId = otherRestaurant.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuCategories.Add(foreignParent);
        await _context.SaveChangesAsync();

        var command = new CreateCategoryCommand(targetRestaurant.Id, new CreateCategoryDto
        {
            Name = "Child",
            DisplayOrder = 1,
            ParentCategoryId = foreignParent.Id
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Parent category not found for this restaurant");
    }

    [Fact]
    public async Task Handle_CreatesCategoryWithParent_WhenValid()
    {
        var restaurant = await SeedRestaurantAsync("Test Restaurant");

        var parentCategory = new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = "Breakfast",
            DisplayOrder = 1,
            RestaurantId = restaurant.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuCategories.Add(parentCategory);
        await _context.SaveChangesAsync();

        var command = new CreateCategoryCommand(restaurant.Id, new CreateCategoryDto
        {
            Name = "Specials",
            DisplayOrder = 2,
            ParentCategoryId = parentCategory.Id
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ParentCategoryId.Should().Be(parentCategory.Id);

        var created = await _context.MenuCategories
            .FirstOrDefaultAsync(c => c.Id == result.Data.Id);

        created.Should().NotBeNull();
        created!.ParentCategoryId.Should().Be(parentCategory.Id);
    }

    private async Task<Restaurant> SeedRestaurantAsync(string name)
    {
        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = $"{name.ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid():N}".Substring(0, 20),
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        return restaurant;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

