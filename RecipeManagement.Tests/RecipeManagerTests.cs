
using System;
using System.Collections.Generic;
using System.Linq;
using RecipeManagement.Core;
using Xunit;

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

    [Fact]
    public void AddToCookingPlan()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(20));
        Assert.True(manager.AddRecipeToCookingPlan(10));

        Assert.Equal(new[] { 20, 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void CookingPlanRejectsDuplicatesAndUnknownIds()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        Assert.False(manager.AddRecipeToCookingPlan(10));
        Assert.False(manager.AddRecipeToCookingPlan(999));
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    public void RemoveFromCookingPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);

        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.False(manager.RemoveRecipeFromCookingPlan(999));
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void EmptyStackIsHandledSafely()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekLastRemovedRecipe());
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void RestoreGoesToEndOfPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.True(manager.RestoreLastRemovedRecipe());

        Assert.Equal(new[] { 20, 10 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void CompleteAllInstructions()
    {
        var manager = CreateManager();
        manager.StartCooking(10);

        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.CompleteNextInstruction());

        Assert.Null(manager.CompleteNextInstruction());
        Assert.Null(manager.PeekNextInstruction());
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCookingUnknownRecipe()
    {
        var manager = CreateManager();

        Assert.False(manager.StartCooking(999));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCookingWithNoInstructions()
    {
        var manager = CreateManager();
        Assert.False(manager.StartCooking(20));
    }

    [Fact]
    public void RecipeStaysAfterCooking()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        manager.StartCooking(10);
        manager.CompleteNextInstruction();
        manager.CompleteNextInstruction();

        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void SearchByTitle_IgnoresCase()
    {
        var manager = CreatePartBManager();
        var results = manager.SearchByTitle("CHICK");

        Assert.Single(results);
        Assert.Equal(10, results[0].Id);
    }

    [Fact]
    public void SearchByTitle_PartialMatch()
    {
        var manager = CreatePartBManager();
        Assert.Single(manager.SearchByTitle("hick"));
    }

    [Fact]
    public void SearchByTitle_NoMatch()
    {
        var manager = CreatePartBManager();
        Assert.Empty(manager.SearchByTitle("zzzzzz"));
    }

    [Fact]
    public void SearchByTitle_BlankText()
    {
        var manager = CreatePartBManager();
        Assert.Equal(manager.RecipeCount, manager.SearchByTitle("   ").Count);
    }

    [Fact]
    public void SearchByIngredient_IgnoresCase()
    {
        var manager = CreatePartBManager();
        var results = manager.SearchByIngredient("EGG");

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Id == 10);
        Assert.Contains(results, r => r.Id == 40);
    }

    [Fact]
    public void SearchByIngredient_NoMatch()
    {
        var manager = CreatePartBManager();
        Assert.Empty(manager.SearchByIngredient("zzzzzz"));
    }

    [Fact]
    public void SearchByIngredient_BlankText()
    {
        var manager = CreatePartBManager();
        Assert.Equal(manager.RecipeCount, manager.SearchByIngredient("").Count);
    }

    [Fact]
    public void GetHighestProteinRecipes_Invalid()
    {
        var manager = CreatePartBManager();
        Assert.Empty(manager.GetHighestProteinRecipes(0));
        Assert.Empty(manager.GetHighestProteinRecipes(-3));
    }

    [Fact]
    public void GetHighestProteinRecipes_ExcludesNoProtein()
    {
        var manager = CreatePartBManager();
        var results = manager.GetHighestProteinRecipes(100);

        Assert.Equal(new[] { 10, 40, 20 }, results.Select(r => r.Id));
    }

    [Fact]
    public void GetHighestProteinRecipes()
    {
        var manager = CreatePartBManager();
        var results = manager.GetHighestProteinRecipes(2);

        Assert.Equal(new[] { 10, 40 }, results.Select(r => r.Id));
    }

    [Fact]
    public void AddSavedRecipe_ExistingId()
    {
        var manager = CreatePartBManager();

        Assert.True(manager.AddSavedRecipe(10));
        Assert.True(manager.IsRecipeSaved(10));
    }

    [Fact]
    public void AddSavedRecipe_Duplicate()
    {
        var manager = CreatePartBManager();
        manager.AddSavedRecipe(10);

        Assert.False(manager.AddSavedRecipe(10));
        Assert.Single(manager.GetSavedRecipes());
    }

    [Fact]
    public void AddSavedRecipe_UnknownId()
    {
        var manager = CreatePartBManager();

        Assert.False(manager.AddSavedRecipe(999));
        Assert.Empty(manager.GetSavedRecipes());
    }

    [Fact]
    public void RemoveSavedRecipe()
    {
        var manager = CreatePartBManager();
        manager.AddSavedRecipe(10);

        Assert.True(manager.RemoveSavedRecipe(10));
        Assert.False(manager.IsRecipeSaved(10));
        Assert.False(manager.RemoveSavedRecipe(10));
    }

    [Fact]
    public void IsRecipeSaved_NotSaved()
    {
        var manager = CreatePartBManager();
        Assert.False(manager.IsRecipeSaved(10));
    }

    [Fact]
    public void GetSavedRecipes_ReturnsAllSavedIds()
    {
        var manager = CreatePartBManager();
        manager.AddSavedRecipe(10);
        manager.AddSavedRecipe(20);

        Assert.Equal(new[] { 10, 20 }, manager.GetSavedRecipes().OrderBy(id => id));
    }

    [Fact]
    public void Search_Save_Remove()
    {
        var manager = CreatePartBManager();

        int id = manager.SearchByTitle("chick").First().Id;

        Assert.True(manager.AddSavedRecipe(id));
        Assert.True(manager.AddRecipeToCookingPlan(id));
        Assert.Equal(new[] { id }, manager.GetCookingPlan());
        Assert.True(manager.RemoveSavedRecipe(id));
        Assert.NotNull(manager.FindRecipe(id));
    }

    [Fact]
    public void RecipeAddedLater_IsFoundBySearch()
    {
        var manager = CreatePartBManager();
        manager.AddRecipe(new Recipe { Id = 50, Title = "Chicken Soup" });

        Assert.Equal(2, manager.SearchByTitle("chicken").Count);
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

    private static RecipeManager CreatePartBManager()
{
    return new RecipeManager(new[]
    {
        new Recipe
        {
            Id = 10,
            Title = "Chicken Curry",
            Ingredients = new() { "200g chicken", "1 Egg" },
            Nutrition = new NutritionInfo
            {
                ProteinG = 40
            }
        },
        new Recipe
        {
            Id = 20,
            Title = "Pasta Bake",
            Ingredients = new() { "pasta", "cheese" },
            Nutrition = new NutritionInfo
            {
                ProteinG = 15
            }
        },
        new Recipe
        {
            Id = 30,
            Title = "Fruit Salad",
            Ingredients = new() { "1 apple" }
        },
        new Recipe
        {
            Id = 40,
            Title = "Egg Fried Rice",
            Ingredients = new() { "rice", "2 eggs", "egg yolk" },
            Nutrition = new NutritionInfo
            {
                ProteinG = 25
            }
        }
    });
}
}
