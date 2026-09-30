using System;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    [Header("Equipment")]
    public ItemData equippedItem;

    [Header("Hand Visual")]
    public Transform handHolder;          // HandHolder
    public ItemData axeItem;              // для определения префаба
    public ItemData pickaxeItem;
    public GameObject axeModelPrefab;
    public GameObject pickaxeModelPrefab;

    private GameObject currentHandModel;

    public event Action<ItemData> OnEquipmentChanged;

    public bool HasItem => equippedItem != null;

    public bool IsEquipped(ItemData item)
    {
        return equippedItem != null && equippedItem == item;
    }

    public void Equip(ItemData item)
    {
        if (item == null) return;

        equippedItem = item;
        SpawnModel(item);

        Debug.Log($"Экипировано: {item.displayName}");
        OnEquipmentChanged?.Invoke(equippedItem);
    }

    public void Unequip()
    {
        if (equippedItem == null) return;

        Debug.Log($"Снято: {equippedItem.displayName}");
        equippedItem = null;
        DestroyModel();

        OnEquipmentChanged?.Invoke(null);
    }

    public void ToggleEquip(ItemData item)
    {
        if (item == null) return;

        if (IsEquipped(item))
            Unequip();
        else
            Equip(item);
    }

    void SpawnModel(ItemData item)
    {
        DestroyModel();

        if (handHolder == null)
        {
            Debug.LogWarning("HandHolder не назначен!");
            return;
        }

        GameObject prefab = null;
        if (item == axeItem) prefab = axeModelPrefab;
        else if (item == pickaxeItem) prefab = pickaxeModelPrefab;

        if (prefab == null)
        {
            Debug.LogWarning($"Нет префаба модели для {item.displayName}");
            return;
        }

        currentHandModel = Instantiate(prefab, handHolder);
        currentHandModel.transform.localPosition = Vector3.zero;
        currentHandModel.transform.localRotation = Quaternion.identity;
    }

    void DestroyModel()
    {
        if (currentHandModel != null)
        {
            Destroy(currentHandModel);
            currentHandModel = null;
        }
    }
}