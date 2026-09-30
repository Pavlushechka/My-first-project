using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeEntryUI : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI label;

    private Recipe recipe;
    private CraftingUI craftingUI;

    public void Setup(Recipe r, CraftingUI ui)
    {
        recipe = r;
        craftingUI = ui;

        if (label != null) label.text = r.result.displayName;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => craftingUI.SelectRecipe(recipe));
        }
    }
}