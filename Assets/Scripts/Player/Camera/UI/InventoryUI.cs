using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform slotsParent;
    public GameObject slotPrefab;

    private Inventory inventory;
    private List<InventorySlotUI> slotPool = new List<InventorySlotUI>();

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory not found on scene");
            return;
        }

        inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (inventory != null) inventory.OnInventoryChanged -= Refresh;
    }

    void Refresh()
    {
        foreach (var slot in slotPool) slot.Clear();

        int count = inventory.items.Count;
        while (slotPool.Count < count)
        {
            var go = Instantiate(slotPrefab, slotsParent);
            slotPool.Add(go.GetComponent<InventorySlotUI>());
        }

        for (int i = 0; i < count; i++)
            slotPool[i].Setup(inventory.items[i].item, inventory.items[i].amount);
    }
}