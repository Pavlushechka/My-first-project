using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildMenuUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("List")]
    public Transform listParent;
    public GameObject buttonPrefab;

    [Header("Buildings")]
    public List<PlaceableObject> availablePlaceables = new List<PlaceableObject>();

    private BuildSystem buildSystem;
    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.ToggleBuildMenu.performed += OnToggleMenu;
    }

    void OnDisable()
    {
        input.Player.ToggleBuildMenu.performed -= OnToggleMenu;
    }

    void Start()
    {
        buildSystem = FindFirstObjectByType<BuildSystem>();

        if (buildSystem == null)
        {
            Debug.LogError("BuildSystem не найден на сцене");
            return;
        }

        BuildList();
        if (panel != null) panel.SetActive(false);
    }

    void OnToggleMenu(InputAction.CallbackContext ctx)
    {
        bool willOpen = panel != null && !panel.activeSelf;

        if (panel != null) panel.SetActive(willOpen);

        Cursor.lockState = willOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = willOpen;
    }

    void BuildList()
    {
        if (listParent == null || buttonPrefab == null) return;

        foreach (var p in availablePlaceables)
        {
            if (p == null) continue;

            var go = Instantiate(buttonPrefab, listParent);
            var btn = go.GetComponent<PlaceableButtonUI>();
            if (btn != null) btn.Setup(p, this);
        }
    }

   public void OnPlaceableSelected(PlaceableObject placeable)
   {
    if (placeable == null) return;

    var inv = FindFirstObjectByType<Inventory>();
    if (inv != null && !inv.HasItems(placeable.requiredItems, placeable.requiredAmounts))
    {
        Debug.Log($"Не хватает ресурсов для {placeable.displayName}");
        return;
    }

    if (panel != null) panel.SetActive(false);
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    if (buildSystem != null)
        buildSystem.StartPlacing(placeable);
    }
}