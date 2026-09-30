using System.Collections.Generic;
using UnityEngine;

public class StorageContainer : Interactable
{
    [Header("Storage")]
    public int capacity = 30;

    public Inventory inventory;

    public event System.Action OnStorageChanged;

    void Awake()
    {
        if (inventory == null)
            inventory = gameObject.AddComponent<Inventory>();

        inventory.capacity = capacity;
    }

    void Start()
    {
        if (string.IsNullOrEmpty(displayName) || displayName == "Объект")
            displayName = "Склад";
    }

    public override void Interact(GameObject interactor)
    {
        Debug.Log($"Открыт склад. Внутри: {inventory.items.Count} видов предметов");

        // Ищем ВКЛЮЧАЯ выключенные объекты
        var ui = FindFirstObjectByType<StorageUI>(FindObjectsInactive.Include);

        if (ui != null)
            ui.Open(this);
        else
            Debug.LogWarning("StorageUI не найден на сцене (даже выключенный)");
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        bool result = inventory.AddItem(item, amount);
        if (result) OnStorageChanged?.Invoke();
        return result;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        bool result = inventory.RemoveItem(item, amount);
        if (result) OnStorageChanged?.Invoke();
        return result;
    }

    // =========================================
    //           СОХРАНЕНИЕ
    // =========================================

    public List<ItemStackSaveData> GetSaveData()
    {
        return inventory.GetSaveData();
    }

    public void ApplySaveData(List<ItemStackSaveData> data)
    {
        inventory.ApplySaveData(data);
    }
}