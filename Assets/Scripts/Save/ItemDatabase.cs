using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("All Items in Game")]
    public List<ItemData> allItems = new List<ItemData>();

    private Dictionary<string, ItemData> lookup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildLookup();
    }

    void BuildLookup()
    {
        lookup = new Dictionary<string, ItemData>();

        foreach (var item in allItems)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.itemId))
            {
                Debug.LogWarning($"[ItemDatabase] У {item.name} пустой itemId!");
                continue;
            }
            if (lookup.ContainsKey(item.itemId))
            {
                Debug.LogWarning($"[ItemDatabase] Дубликат itemId: {item.itemId}");
                continue;
            }
            lookup[item.itemId] = item;
        }

        Debug.Log($"[ItemDatabase] Загружено {lookup.Count} предметов");
    }

    public ItemData GetById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (lookup == null) BuildLookup();

        if (lookup.TryGetValue(id, out var item))
            return item;

        Debug.LogWarning($"[ItemDatabase] Предмет с id='{id}' не найден");
        return null;
    }
}