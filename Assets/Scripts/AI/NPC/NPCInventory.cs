using UnityEngine;

public class NPCInventory : MonoBehaviour
{
    [Header("Carrying")]
    public int maxCarry = 10;
    public int currentCarry = 0;
    public ItemData carriedItem;

    public bool HasItems => currentCarry > 0;
    public bool IsFull => currentCarry >= maxCarry;

    public bool Pickup(ItemData item, int amount = 1)
    {
        if (item == null) return false;
        if (IsFull) return false;

        // NPC носит только один тип ресурса за раз
        if (carriedItem != null && carriedItem != item) return false;

        carriedItem = item;
        currentCarry = Mathf.Min(currentCarry + amount, maxCarry);

        Debug.Log($"[NPC] Взял {item.displayName} x{currentCarry}/{maxCarry}");
        return true;
    }

    public int DropAll()
    {
        int amount = currentCarry;
        carriedItem = null;
        currentCarry = 0;
        return amount;
    }
    public void ApplySaveData(string itemId, int amount)
    {
    carriedItem = null;
    currentCarry = 0;

    if (string.IsNullOrEmpty(itemId) || amount <= 0) return;

    var item = ItemDatabase.Instance.GetById(itemId);
    if (item == null)
    {
        Debug.LogWarning($"[NPCInventory] Предмет '{itemId}' не найден");
        return;
    }

    carriedItem = item;
    currentCarry = amount;
    }
}