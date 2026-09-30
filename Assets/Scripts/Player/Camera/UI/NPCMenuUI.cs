using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCMenuUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("List")]
    public Transform listParent;
    public GameObject entryPrefab;

    [Header("Debug")]
    public bool debugLogs = true;

    private PlayerInputActions input;
    private bool isOpen = false;
    private List<NPCEntryUI> entries = new List<NPCEntryUI>();

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.UI.Enable();
        input.UI.ToggleNPCMenu.performed += OnToggleMenu;
    }

    void OnDisable()
    {
        input.UI.ToggleNPCMenu.performed -= OnToggleMenu;
    }

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    void OnToggleMenu(InputAction.CallbackContext ctx)
    {
        isOpen = !isOpen;

        if (panel != null) panel.SetActive(isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (isOpen)
            RebuildList();

        if (debugLogs)
            Debug.Log($"[NPCMenu] Меню {(isOpen ? "открыто" : "закрыто")}");
    }

    void RebuildList()
    {
        // 1. Удаляем старые строки
        foreach (var entry in entries)
        {
            if (entry != null)
                Destroy(entry.gameObject);
        }
        entries.Clear();

        if (listParent == null || entryPrefab == null)
        {
            Debug.LogError("[NPCMenu] listParent или entryPrefab не назначены!");
            return;
        }

        // 2. Находим всех NPC в сцене
        var npcs = FindObjectsByType<NPCBrain>(FindObjectsSortMode.None);

        // 3. Сортируем по npcId
        System.Array.Sort(npcs, (a, b) => a.npcId.CompareTo(b.npcId));

        // 4. Создаём строки
        foreach (var npc in npcs)
        {
            var go = Instantiate(entryPrefab, listParent);
            var entry = go.GetComponent<NPCEntryUI>();

            if (entry != null)
            {
                entry.Setup(npc);
                entries.Add(entry);
            }
        }

        if (debugLogs)
            Debug.Log($"[NPCMenu] Обновлён список. NPC: {npcs.Length}");
    }
}