using FluentAssertions;
using Menufy.Application.Features.Menus.Helpers;
using Menufy.Domain.Entities;
using Xunit;

namespace Menufy.Application.Tests.Menus;

public class MenuCategoryTreeBuilderTests
{
    [Fact]
    public void BuildTree_ReturnsNestedStructureOrderedByDisplayOrder()
    {
        var rootCategory = new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = "Drinks",
            DisplayOrder = 1,
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "Espresso", DisplayOrder = 2, Price = 3.5m, IsAvailable = true },
                new MenuItem { Id = Guid.NewGuid(), Name = "Americano", DisplayOrder = 1, Price = 4.0m, IsAvailable = true }
            }
        };

        var childCategory = new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = "Specials",
            DisplayOrder = 2,
            ParentCategoryId = rootCategory.Id,
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "Pumpkin Spice Latte", DisplayOrder = 1, Price = 5.25m, IsAvailable = true }
            }
        };

        var siblingCategory = new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = "Food",
            DisplayOrder = 0,
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "Croissant", DisplayOrder = 1, Price = 2.75m, IsAvailable = true }
            }
        };

        var categories = new List<MenuCategory> { rootCategory, childCategory, siblingCategory };

        var tree = MenuCategoryTreeBuilder.BuildTree(categories);

        tree.Should().HaveCount(2);
        tree.Select(c => c.Name).Should().Equal("Food", "Drinks");

        var drinks = tree.Single(c => c.Name == "Drinks");
        drinks.Children.Should().HaveCount(1);
        drinks.Children[0].Name.Should().Be("Specials");

        drinks.Items.Should().HaveCount(2);
        drinks.Items.Select(i => i.Name).Should().Equal("Americano", "Espresso");
    }

    [Fact]
    public void BuildTree_FiltersItemsUsingPredicate()
    {
        var category = new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = "Breakfast",
            DisplayOrder = 0,
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "Pancakes", DisplayOrder = 0, Price = 7.5m, IsAvailable = true },
                new MenuItem { Id = Guid.NewGuid(), Name = "Omelette", DisplayOrder = 1, Price = 8.0m, IsAvailable = false }
            }
        };

        var tree = MenuCategoryTreeBuilder.BuildTree(new[] { category }, item => item.IsAvailable);

        tree.Should().ContainSingle();
        tree[0].Items.Should().ContainSingle();
        tree[0].Items[0].Name.Should().Be("Pancakes");
    }
}

