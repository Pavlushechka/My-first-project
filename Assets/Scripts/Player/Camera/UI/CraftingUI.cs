using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("List")]
    public Transform recipesParent;
    public GameObject recipeEntryPrefab;

    [Header("Detail")]
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailIngredients;
    public Button craftButton;

    private CraftingSystem crafting;
    private Inventory inventory;
    private Recipe selectedRecipe;
    private List<RecipeEntryUI> entries = new List<RecipeEntryUI>();
    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.ToggleCrafting.performed += OnToggleCrafting;
    }

    void OnDisable()
    {
        input.Player.ToggleCrafting.performed -= OnToggleCrafting;
    }

    void Start()
    {
        crafting = FindFirstObjectByType<CraftingSystem>();
        inventory = FindFirstObjectByType<Inventory>();

        if (crafting == null || inventory == null)
        {
            Debug.LogError("CraftingSystem или Inventory не найдены!");
            return;
        }

        BuildList();
        if (panel != null) panel.SetActive(false);

        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftClicked);
    }

    void OnDestroy()
    {
        if (craftButton != null)
            craftButton.onClick.RemoveListener(OnCraftClicked);
    }

    void OnToggleCrafting(InputAction.CallbackContext ctx)
    {
        bool willOpen = panel != null && !panel.activeSelf;

        if (panel != null) panel.SetActive(willOpen);

        Cursor.lockState = willOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = willOpen;
    }

    void BuildList()
    {
        if (recipesParent == null || recipeEntryPrefab == null || crafting == null) return;

        foreach (var r in crafting.knownRecipes)
        {
            var go = Instantiate(recipeEntryPrefab, recipesParent);
            var entry = go.GetComponent<RecipeEntryUI>();
            if (entry == null) continue;
            entry.Setup(r, this);
            entries.Add(entry);
        }
    }

    public void SelectRecipe(Recipe recipe)
    {
        selectedRecipe = recipe;
        RefreshDetails();
    }

    void RefreshDetails()
    {
        if (selectedRecipe == null) return;

        if (detailName != null)
            detailName.text = selectedRecipe.result.displayName;

        if (detailIngredients != null)
        {
            string s = "";
            foreach (var ing in selectedRecipe.ingredients)
            {
                int have = inventory.GetAmount(ing.item);
                string color = have >= ing.amount ? "white" : "red";
                s += $"<color={color}>{ing.item.displayName}: {have}/{ing.amount}</color>\n";
            }
            detailIngredients.text = s;
        }

        if (craftButton != null)
            craftButton.interactable = crafting.CanCraft(selectedRecipe);
    }

    void OnCraftClicked()
    {
        if (selectedRecipe == null) return;
        crafting.Craft(selectedRecipe);
        RefreshDetails();
    }
}