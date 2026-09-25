using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{

    Dictionary<int, Recipe> recipes = new Dictionary<int, Recipe>();
    List<string> shoppingList = new List<string>();
    
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        ArgumentNullException.ThrowIfNull(recipes);

        foreach (Recipe recipe in recipes)
        {
            if (recipe is null)
            {
                throw new ArgumentException ("Recipe information cannot be null", nameof(recipes));
            }

            if (!this.recipes.TryAdd(recipe.Id, recipe))
                {
                    throw new ArgumentException($"Duplicate recipe ID {recipe.Id}", nameof(recipes));
                }
        }
    }

    public int RecipeCount => recipes.Count;
    public int ShoppingItemCount => shoppingList.Count;
    public int CookingPlanCount => 0;
    public int PendingInstructionCount => 0;
    public int RemovedRecipeCount => 0;

    public bool AddRecipe(Recipe recipe)
    {
        return recipes.TryAdd(recipe.Id, recipe);
    }

    public Recipe? FindRecipe(int recipeId)
    {
        if (recipes.ContainsKey(recipeId))
        {
            return recipes[recipeId];
        }
        else
        {
            return null;
        }
    }

    public bool RemoveRecipe(int recipeId)
    {
            return recipes.Remove(recipeId);
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        if (!recipes.ContainsKey(recipeId))
        {
            return 0;
        }

        Recipe recipe = recipes[recipeId];

        foreach (string ingredient in recipe.Ingredients)
        {
            shoppingList.Add(ingredient);
        }

        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return shoppingList;
    }

    public void ClearShoppingList() 
    {
        shoppingList.Clear();
    }

    public bool AddRecipeToCookingPlan(int recipeId) =>
        throw new NotImplementedException("Part A: implement AddRecipeToCookingPlan.");

    public bool RemoveRecipeFromCookingPlan(int recipeId) =>
        throw new NotImplementedException("Part A: implement RemoveRecipeFromCookingPlan.");

    public bool RestoreLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement RestoreLastRemovedRecipe.");

    public int? PeekLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement PeekLastRemovedRecipe.");

    public IReadOnlyList<int> GetCookingPlan() =>
        throw new NotImplementedException("Part A: implement GetCookingPlan.");

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Part A: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Part A: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Part A: implement CompleteNextInstruction.");

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
