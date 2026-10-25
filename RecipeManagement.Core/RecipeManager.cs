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
    LinkedList<int> cookingPlan = new LinkedList<int>();
    Stack<int> removedRecipes = new Stack<int>();
    Queue<string> instructions = new Queue<string>();
    HashSet<int> savedRecepies = New HashSet<int>();
    
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

            if (recipe.Id <= 0)
            {
                throw new ArgumentException("Recipe ID not valid", nameof(recipes));
            }

            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Recipe name can't be empty", nameof(recipes));
            }
        }
    }

    public int RecipeCount => recipes.Count;
    public int ShoppingItemCount => shoppingList.Count;
    public int CookingPlanCount => cookingPlan.Count;
    public int PendingInstructionCount => instructions.Count;
    public int RemovedRecipeCount => removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    { 
        ArgumentNullException.ThrowIfNull(recipe);

        if (recipe.Id <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false;
        }

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
        if (cookingPlan.Contains(recipeId))
        {
            return false;
        }
 
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
        return new List<string>(shoppingList);
    }

    public void ClearShoppingList() 
    {
        shoppingList.Clear();
    }

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!recipes.ContainsKey(recipeId))
        {
            return false;
        }
 
        if (cookingPlan.Contains(recipeId))
        {
            return false;
        }
 
        cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId) 
    {
        if (cookingPlan.Remove(recipeId))
        {
            removedRecipes.Push(recipeId);
            return true;
        }

        return false;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (removedRecipes.Count == 0)
        {
            return false;
        }
 
        int recipeId = removedRecipes.Pop();
 
        if (cookingPlan.Contains(recipeId))
        {
            return false;
        }

        if (!recipes.ContainsKey(recipeId))
        {
           return false;
        }
 
        cookingPlan.AddLast(recipeId);
        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (removedRecipes.Count == 0)
        {
            return null;
        }

        return removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        List<int> plan = new List<int>();
 
        foreach (int id in cookingPlan)
        {
            plan.Add(id);
        }
 
        return plan;
    }

    public bool StartCooking(int recipeId) 
    {
        if (!recipes.ContainsKey(recipeId))
        {
            return false;
        }
 
        Recipe recipe = recipes[recipeId];

        if (recipe.Instructions.Count == 0)
        {
            return false;
        }

        instructions.Clear();
 
        foreach (string step in recipes[recipeId].Instructions)
        {
            instructions.Enqueue(step);
        }

        return true;
    }

    public string? PeekNextInstruction() 
    {
        if (instructions.Count == 0)
        {
            return null;
        }
 
        return instructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (instructions.Count == 0)
        {
            return null;
        }
 
        return instructions.Dequeue();
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) 
    {
        if (string.IsNullOrWhiteSpace(searchText)){
            return recipes.Values.ToList();
        }

        return (from r in recipes.Values where r.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) select r).ToList();
    }

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)){
            return recipes.Values.ToList();
        }

        return (from r in recipies.Values where r.Ingredients.Any(i => i.Contains(searchText, StringComparison.OrdinalIgnoreCase))select r).ToList();
    }

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count)
    {
        if (count <= 0)
        {
            return new List<Recipe>();
        }

        return (from r in recipe.Values where r.Protein.HasValue orderby r.Protein descending select r).Take(count).ToList();
    }

    public bool AddSavedRecipe(int recipeId) 
    {
        if (!recipes.ContainsKey(recipeId))
        {
            return false;
        }

        return savedRecipes.Add(recipeId);
    }

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
