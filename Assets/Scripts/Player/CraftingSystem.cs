using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public List<Recipe> knownRecipes = new List<Recipe>();

    private Inventory inventory;

    void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    public bool CanCraft(Recipe recipe)
    {
        if (recipe == null || inventory == null) return false;

        foreach (var ing in recipe.ingredients)
        {
            if (ing.item == null) continue;
            if (inventory.GetAmount(ing.item) < ing.amount)
                return false;
        }
        return true;
    }

    public bool Craft(Recipe recipe)
    {
        if (!CanCraft(recipe))
        {
            Debug.Log($"Не хватает ресурсов для {recipe.name}");
            return false;
        }

        // Списываем ингредиенты
        foreach (var ing in recipe.ingredients)
        {
            if (ing.item == null) continue;
            inventory.RemoveItem(ing.item, ing.amount);
        }

        // Выдаём результат
        inventory.AddItem(recipe.result, recipe.resultAmount);
        Debug.Log($"Создано: {recipe.result.displayName} x{recipe.resultAmount}");
        return true;
    }
}