using UnityEngine;
using UnityEngine.InputSystem;

public class DebugGrantResources : MonoBehaviour
{
    public ItemData[] items;
    public int[] amounts;

    void Start()
    {
        Grant();
    }

    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
          Grant();
    }

    void Grant()
    {
        var inv = GetComponent<Inventory>();
        if (inv == null)
        {
            Debug.LogWarning("Inventory не найден");
            return;
        }

        for (int i = 0; i < items.Length && i < amounts.Length; i++)
        {
            if (items[i] == null) continue;
            inv.AddItem(items[i], amounts[i]);
            Debug.Log($"[DEBUG] Выдано: {items[i].displayName} x{amounts[i]}");
        }
    }
}