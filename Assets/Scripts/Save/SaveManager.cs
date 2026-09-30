using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("References")]
    public Transform playerTransform;
    public CharacterController characterController;
    public PlayerStats playerStats;
    public Inventory playerInventory;
    public EquipmentSlot equipmentSlot;
    public Transform cameraTransform;

    [Header("Settings")]
    public int defaultSlot = 1;

    [Header("Debug")]
    public bool debugLogs = true;

    private PlayerInputActions input;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.UI.Enable();
        input.UI.Save.performed += OnSave;
        input.UI.Load.performed += OnLoad;
    }

    void OnDisable()
    {
        input.UI.Save.performed -= OnSave;
        input.UI.Load.performed -= OnLoad;
    }

    void OnSave(InputAction.CallbackContext ctx) => SaveGame(defaultSlot);
    void OnLoad(InputAction.CallbackContext ctx) => LoadGame(defaultSlot);

    public void SaveGame(int slot)
    {
        var data = new SaveData();

        CollectPlayerData(data);
        CollectBuildings(data);
        CollectNPCs(data);
        CollectDestroyedNodes(data);

        SaveSystem.Save(data, slot);

        if (debugLogs)
            Debug.Log($"[SaveManager] Сохранено в слот {slot}. Зданий: {data.buildings.Count}, NPC: {data.npcs.Count}, Срублено: {data.destroyedNodes.Count}");
    }

    public void LoadGame(int slot)
    {
        var data = SaveSystem.Load(slot);

        if (data == null)
        {
            Debug.LogWarning($"[SaveManager] Не удалось загрузить слот {slot}");
            return;
        }

        ApplyPlayerData(data);
        ApplyBuildings(data);
        ApplyNPCs(data);
        ApplyDestroyedNodes(data);

        if (debugLogs)
            Debug.Log($"[SaveManager] Загружено из слота {slot}. Зданий: {data.buildings.Count}, NPC: {data.npcs.Count}, Срублено: {data.destroyedNodes.Count}");
    }

    // =========================================
    //           ИГРОК
    // =========================================

    void CollectPlayerData(SaveData data)
    {
        if (playerTransform != null)
        {
            data.player.posX = playerTransform.position.x;
            data.player.posY = playerTransform.position.y;
            data.player.posZ = playerTransform.position.z;
            data.player.rotY = playerTransform.eulerAngles.y;
        }

        if (playerStats != null)
        {
            data.player.health = playerStats.currentHealth;
            data.player.hunger = playerStats.currentHunger;
            data.player.thirst = playerStats.currentThirst;
        }

        if (playerInventory != null)
            data.inventory = playerInventory.GetSaveData();

        if (equipmentSlot != null && equipmentSlot.equippedItem != null)
            data.player.equippedItemId = equipmentSlot.equippedItem.itemId;
        else
            data.player.equippedItemId = "";
    }

    void ApplyPlayerData(SaveData data)
    {
        if (playerTransform != null)
        {
            if (characterController != null) characterController.enabled = false;

            playerTransform.position = new Vector3(data.player.posX, data.player.posY, data.player.posZ);
            playerTransform.rotation = Quaternion.Euler(0f, data.player.rotY, 0f);

            if (characterController != null) characterController.enabled = true;
        }

        if (playerStats != null)
        {
            playerStats.currentHealth = data.player.health;
            playerStats.currentHunger = data.player.hunger;
            playerStats.currentThirst = data.player.thirst;
        }

        if (playerInventory != null && data.inventory != null)
            playerInventory.ApplySaveData(data.inventory);

        if (equipmentSlot != null)
        {
            var item = string.IsNullOrEmpty(data.player.equippedItemId)
                ? null
                : ItemDatabase.Instance.GetById(data.player.equippedItemId);

            if (item != null)
                equipmentSlot.Equip(item);
            else
                equipmentSlot.Unequip();
        }
    }

    // =========================================
    //           ПОСТРОЙКИ
    // =========================================

    void CollectBuildings(SaveData data)
    {
        data.buildings.Clear();

        var storages = FindObjectsByType<StorageContainer>(FindObjectsSortMode.None);

        foreach (var storage in storages)
        {
            var buildingData = new BuildingSaveData();

            buildingData.buildingTypeId = "warehouse";

            buildingData.posX = storage.transform.position.x;
            buildingData.posY = storage.transform.position.y;
            buildingData.posZ = storage.transform.position.z;
            buildingData.rotY = storage.transform.eulerAngles.y;

            buildingData.storageItems = storage.GetSaveData();

            data.buildings.Add(buildingData);
        }

        if (debugLogs)
            Debug.Log($"[SaveManager] Собрано зданий: {data.buildings.Count}");
    }

    void ApplyBuildings(SaveData data)
    {
        var existing = FindObjectsByType<StorageContainer>(FindObjectsSortMode.None);

        foreach (var storage in existing)
            Destroy(storage.gameObject);

        if (debugLogs)
            Debug.Log($"[SaveManager] Удалено зданий: {existing.Length}");

        if (data.buildings == null) return;

        foreach (var b in data.buildings)
        {
            var placeable = BuildingDatabase.Instance.GetById(b.buildingTypeId);

            if (placeable == null || placeable.prefab == null)
            {
                Debug.LogWarning($"[SaveManager] Не найден префаб для '{b.buildingTypeId}'");
                continue;
            }

            Vector3 pos = new Vector3(b.posX, b.posY, b.posZ);
            Quaternion rot = Quaternion.Euler(0f, b.rotY, 0f);

            var obj = Instantiate(placeable.prefab, pos, rot);
            obj.name = $"Warehouse(Clone)_restored";

            var storage = obj.GetComponent<StorageContainer>();
            if (storage != null && b.storageItems != null)
                storage.ApplySaveData(b.storageItems);
        }

        if (debugLogs)
            Debug.Log($"[SaveManager] Восстановлено зданий: {data.buildings.Count}");
    }

    // =========================================
    //             NPC
    // =========================================

    void CollectNPCs(SaveData data)
    {
        data.npcs.Clear();

        var npcs = FindObjectsByType<NPCSaveable>(FindObjectsSortMode.None);

        foreach (var npc in npcs)
        {
            var npcData = new NPCSaveData();

            npcData.posX = npc.transform.position.x;
            npcData.posY = npc.transform.position.y;
            npcData.posZ = npc.transform.position.z;

            var stats = npc.GetComponent<NPCStats>();
            if (stats != null)
            {
                npcData.hunger = stats.currentHunger;
                npcData.energy = stats.currentEnergy;
            }

            var inv = npc.GetComponent<NPCInventory>();
            if (inv != null && inv.carriedItem != null)
            {
                npcData.carriedItemId = inv.carriedItem.itemId;
                npcData.carriedAmount = inv.currentCarry;
            }
            else
            {
                npcData.carriedItemId = "";
                npcData.carriedAmount = 0;
            }

            data.npcs.Add(npcData);
        }

        if (debugLogs)
            Debug.Log($"[SaveManager] Собрано NPC: {data.npcs.Count}");
    }

    void ApplyNPCs(SaveData data)
    {
        var existing = FindObjectsByType<NPCSaveable>(FindObjectsSortMode.None);

        foreach (var npc in existing)
            Destroy(npc.gameObject);

        if (debugLogs)
            Debug.Log($"[SaveManager] Удалено NPC: {existing.Length}");

        if (data.npcs == null) return;

        var spawner = FindFirstObjectByType<NPCSpawner>();
        if (spawner == null || spawner.npcPrefab == null)
        {
            Debug.LogWarning("[SaveManager] NPCSpawner не найден или без префаба");
            return;
        }

        foreach (var n in data.npcs)
        {
            Vector3 pos = new Vector3(n.posX, n.posY, n.posZ);
            var obj = Instantiate(spawner.npcPrefab, pos, Quaternion.identity);
            obj.name = "NPC_Peasant(Clone)_restored";

            var stats = obj.GetComponent<NPCStats>();
            if (stats != null)
                stats.ApplySaveData(n.hunger, n.energy);

            var inv = obj.GetComponent<NPCInventory>();
            if (inv != null)
                inv.ApplySaveData(n.carriedItemId, n.carriedAmount);
        }

        if (debugLogs)
            Debug.Log($"[SaveManager] Восстановлено NPC: {data.npcs.Count}");
    }

    // =========================================
    //        СОСТОЯНИЕ МИРА (срубленные ноды)
    // =========================================

    void CollectDestroyedNodes(SaveData data)
    {
        data.destroyedNodes = new List<DestroyedNodeSaveData>(DestroyedNodesRegistry.destroyed);

        if (debugLogs)
            Debug.Log($"[SaveManager] Собрано срубленных нод: {data.destroyedNodes.Count}");
    }

    void ApplyDestroyedNodes(SaveData data)
    {
        DestroyedNodesRegistry.ReplaceWith(data.destroyedNodes);

        if (debugLogs)
            Debug.Log($"[SaveManager] Загружено срубленных нод в реестр: {DestroyedNodesRegistry.destroyed.Count}");

        var allNodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);

        int removed = 0;

        foreach (var node in allNodes)
        {
            if (IsDestroyed(node))
            {
                Destroy(node.gameObject);
                removed++;
            }
        }

        if (debugLogs)
            Debug.Log($"[SaveManager] Удалено восстановленных нод: {removed}");
    }

    bool IsDestroyed(ResourceNode node)
    {
        Vector3 pos = node.transform.position;

        foreach (var d in DestroyedNodesRegistry.destroyed)
        {
            if (d.nodeTypeId != node.nodeTypeId) continue;

            float dx = Mathf.Abs(pos.x - d.posX);
            float dy = Mathf.Abs(pos.y - d.posY);
            float dz = Mathf.Abs(pos.z - d.posZ);

            if (dx < 0.5f && dy < 0.5f && dz < 0.5f)
                return true;
        }

        return false;
    }
}