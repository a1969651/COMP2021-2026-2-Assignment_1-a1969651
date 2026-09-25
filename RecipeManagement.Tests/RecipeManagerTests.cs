using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void DuplicateIds()
    {
        var duplicates = new[]
        {
            new Recipe { Id = 10, Title = "A" },
            new Recipe { Id = 10, Title = "B" }
        };
 
        Assert.Throws<ArgumentException>(() => new RecipeManager(duplicates));
    }
 
    [Fact]
    public void AddRecipe()
    {
        var manager = CreateManager();
 
        bool add = manager.AddRecipe(new Recipe { Id = 30, Title = "Recipe C" });
 
        Assert.True(add);
        Assert.Equal(3, manager.RecipeCount);
    }
 
    [Fact]
    public void FindRecipe()
    {
        var manager = CreateManager();
 
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
        Assert.Null(manager.FindRecipe(999));
    }
 
    [Fact]
    public void RemoveRecipe()
    {
        var manager = CreateManager();
 
        Assert.True(manager.RemoveRecipe(10));
        Assert.False(manager.RemoveRecipe(999));
        Assert.Equal(1, manager.RecipeCount);
    }
 
    [Fact]
    public void AddIngredientsToShoppingList()
    {
        var manager = CreateManager();
 
        int add = manager.AddIngredientsToShoppingList(10);
 
        Assert.Equal(1, add);
        Assert.Equal(new[] { "1 apple" }, manager.GetShoppingList());
    }
 
    [Fact]
    public void ClearShoppingList()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
 
        manager.ClearShoppingList();
 
        Assert.Empty(manager.GetShoppingList());
    }


    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
