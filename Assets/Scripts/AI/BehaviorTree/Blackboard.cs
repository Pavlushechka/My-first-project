using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public void Set<T>(string key, T value)
    {
        data[key] = value;
    }

    public T Get<T>(string key)
    {
        if (!data.ContainsKey(key)) return default;
        if (data[key] is T typed) return typed;
        return default;
    }

    public bool Has(string key)
    {
        return data.ContainsKey(key);
    }

    public void Remove(string key)
    {
        if (data.ContainsKey(key))
            data.Remove(key);
    }

    public void Clear()
    {
        data.Clear();
    }

    public void DebugDump()
    {
        Debug.Log($"=== Blackboard ({data.Count} записей) ===");
        foreach (var kvp in data)
            Debug.Log($"  {kvp.Key} = {kvp.Value}");
    }
}