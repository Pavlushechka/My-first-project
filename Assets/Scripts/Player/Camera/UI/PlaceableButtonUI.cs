using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceableButtonUI : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI label;

    private PlaceableObject placeable;
    private BuildMenuUI menu;

    public void Setup(PlaceableObject p, BuildMenuUI m)
    {
        placeable = p;
        menu = m;

        if (label != null)
            label.text = p.displayName;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => menu.OnPlaceableSelected(placeable));
        }
    }

    void OnEnable()
    {
        RefreshInteractable();
    }

    void Update()
    {
        RefreshInteractable();
    }

    void RefreshInteractable()
    {
        if (button == null || placeable == null) return;

        var inv = FindFirstObjectByType<Inventory>();
        if (inv == null) return;

        button.interactable = inv.HasItems(placeable.requiredItems, placeable.requiredAmounts);
    }
}