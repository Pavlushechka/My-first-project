using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStack
{
    public ItemData item;
    public int amount;

    public ItemStack(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}

public class Inventory : MonoBehaviour
{
    public int capacity = 20;

    public List<ItemStack> items = new List<ItemStack>();

    public event Action OnInventoryChanged;

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // Сначала пытаемся доложить в существующий стак
        foreach (var stack in items)
        {
            if (stack.item == item && stack.amount < item.maxStack)
            {
                int space = item.maxStack - stack.amount;
                int toAdd = Mathf.Min(space, amount);
                stack.amount += toAdd;
                amount -= toAdd;

                if (amount <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        // Потом создаём новые стаки
        while (amount > 0 && items.Count < capacity)
        {
            int toAdd = Mathf.Min(item.maxStack, amount);
            items.Add(new ItemStack(item, toAdd));
            amount -= toAdd;
        }

        OnInventoryChanged?.Invoke();
        return amount <= 0;
    }

    public int GetAmount(ItemData item)
    {
        int total = 0;
        foreach (var stack in items)
            if (stack.item == item) total += stack.amount;
        return total;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (GetAmount(item) < amount) return false;

        for (int i = items.Count - 1; i >= 0 && amount > 0; i--)
        {
            if (items[i].item != item) continue;

            int toRemove = Mathf.Min(items[i].amount, amount);
            items[i].amount -= toRemove;
            amount -= toRemove;

            if (items[i].amount <= 0)
                items.RemoveAt(i);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }
    public bool HasItems(ItemData[] items, int[] amounts)
    {
    if (items == null || amounts == null) return true;
    if (items.Length != amounts.Length) return false;

    for (int i = 0; i < items.Length; i++)
    {
        if (items[i] == null) continue;
        if (GetAmount(items[i]) < amounts[i]) return false;
    }
    return true;
    }

    public bool RemoveItems(ItemData[] items, int[] amounts)
    {
    if (items == null || amounts == null) return true;
    if (items.Length != amounts.Length) return false;

    if (!HasItems(items, amounts)) return false;

    for (int i = 0; i < items.Length; i++)
    {
        if (items[i] == null) continue;
        RemoveItem(items[i], amounts[i]);
    }
    return true;
    }
    public List<ItemStackSaveData> GetSaveData()
    {
    var result = new List<ItemStackSaveData>();

    foreach (var stack in items)
    {
        if (stack.item == null) continue;

        result.Add(new ItemStackSaveData
        {
            itemId = stack.item.itemId,
            amount = stack.amount
        });
    }

    return result;
    }

    public void ApplySaveData(List<ItemStackSaveData> data)
    {
    items.Clear();

    if (data == null) return;

    foreach (var stack in data)
    {
        var item = ItemDatabase.Instance.GetById(stack.itemId);
        if (item == null)
        {
            Debug.LogWarning($"[Inventory] Предмет с id='{stack.itemId}' не найден");
            continue;
        }

        items.Add(new ItemStack(item, stack.amount));
    }

    OnInventoryChanged?.Invoke();
    }
}