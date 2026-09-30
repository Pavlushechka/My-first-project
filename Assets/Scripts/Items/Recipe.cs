using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ingredient
{
    public ItemData item;
    public int amount = 1;
}

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Game/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Result")]
    public ItemData result;
    public int resultAmount = 1;

    [Header("Ingredients")]
    public List<Ingredient> ingredients = new List<Ingredient>();
}