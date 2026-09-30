using System.Collections.Generic;
using UnityEngine;

public class BuildingDatabase : MonoBehaviour
{
    public static BuildingDatabase Instance { get; private set; }

    [Header("All Building Types")]
    public List<PlaceableObject> allPlaceables = new List<PlaceableObject>();

    private Dictionary<string, PlaceableObject> lookup;

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
        lookup = new Dictionary<string, PlaceableObject>();

        foreach (var p in allPlaceables)
        {
            if (p == null) continue;
            if (string.IsNullOrEmpty(p.buildingTypeId))
            {
                Debug.LogWarning($"[BuildingDatabase] У {p.name} пустой buildingTypeId!");
                continue;
            }
            if (lookup.ContainsKey(p.buildingTypeId))
            {
                Debug.LogWarning($"[BuildingDatabase] Дубликат buildingTypeId: {p.buildingTypeId}");
                continue;
            }
            lookup[p.buildingTypeId] = p;
        }

        Debug.Log($"[BuildingDatabase] Загружено {lookup.Count} типов построек");
    }

    public PlaceableObject GetById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (lookup == null) BuildLookup();

        if (lookup.TryGetValue(id, out var placeable))
            return placeable;

        Debug.LogWarning($"[BuildingDatabase] Постройка с id='{id}' не найдена");
        return null;
    }
}