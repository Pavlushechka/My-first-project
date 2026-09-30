using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StorageUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Player Side")]
    public Transform playerSlotsParent;
    public GameObject slotPrefab;

    [Header("Storage Side")]
    public Transform storageSlotsParent;

    private StorageContainer currentContainer;
    private Inventory playerInventory;
    private List<StorageSlotUI> playerSlots = new List<StorageSlotUI>();
    private List<StorageSlotUI> storageSlots = new List<StorageSlotUI>();
    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.UI.Enable();
        input.UI.Cancel.performed += OnCancel;
    }

    void OnDisable()
    {
        input.UI.Cancel.performed -= OnCancel;
    }

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        if (panel != null && panel.activeSelf)
            Close();
    }

    public void Open(StorageContainer container)
    {
        Debug.Log($"[StorageUI] Open вызван. container={(container == null ? "NULL" : container.name)}");

        if (container == null) return;

        currentContainer = container;

        // Ищем инвентарь игрока через PlayerInteractor (он только на игроке)
        var interactor = FindFirstObjectByType<PlayerInteractor>();
        Debug.Log($"[StorageUI] PlayerInteractor найден: {(interactor == null ? "НЕТ" : interactor.gameObject.name)}");

        if (interactor != null)
            playerInventory = interactor.GetComponent<Inventory>();

        if (playerInventory == null)
        {
            Debug.LogError("[StorageUI] Inventory игрока не найден!");
            return;
        }

        Debug.Log($"[StorageUI] playerInventory найден на {playerInventory.gameObject.name}, items={playerInventory.items.Count}");

        if (panel != null) panel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Refresh();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentContainer = null;
    }

    void Refresh()
    {
        if (currentContainer == null || playerInventory == null) return;

        Debug.Log($"[StorageUI] Refresh. Player items: {playerInventory.items.Count}, Storage items: {currentContainer.inventory.items.Count}");
        Debug.Log($"[StorageUI] slotPrefab={(slotPrefab == null ? "NULL" : slotPrefab.name)}, playerParent={(playerSlotsParent == null ? "NULL" : playerSlotsParent.name)}, storageParent={(storageSlotsParent == null ? "NULL" : storageSlotsParent.name)}");

        foreach (var s in playerSlots) Destroy(s.gameObject);
        foreach (var s in storageSlots) Destroy(s.gameObject);

        playerSlots.Clear();
        storageSlots.Clear();

        foreach (var stack in playerInventory.items)
            CreateSlot(playerSlotsParent, stack, playerSlots, true);

        foreach (var stack in currentContainer.inventory.items)
            CreateSlot(storageSlotsParent, stack, storageSlots, false);

        Debug.Log($"[StorageUI] Создано слотов: player={playerSlots.Count}, storage={storageSlots.Count}");
    }

    void CreateSlot(Transform parent, ItemStack stack, List<StorageSlotUI> list, bool isPlayerSide)
    {
        if (parent == null || slotPrefab == null)
        {
            Debug.LogWarning($"[StorageUI] CreateSlot пропущен. parent={(parent == null ? "NULL" : parent.name)}, prefab={(slotPrefab == null ? "NULL" : slotPrefab.name)}");
            return;
        }

        var go = Instantiate(slotPrefab, parent);
        var slot = go.GetComponent<StorageSlotUI>();

        if (slot != null)
        {
            slot.Setup(stack.item, stack.amount, this, isPlayerSide);
            list.Add(slot);
            Debug.Log($"[StorageUI] Создан слот: {stack.item.displayName} x{stack.amount}, сторона: {(isPlayerSide ? "игрок" : "склад")}");
        }
        else
        {
            Debug.LogError("[StorageUI] У префаба слота нет компонента StorageSlotUI");
        }
    }

    public void TransferFromPlayerToStorage(ItemData item)
    {
        if (item == null || currentContainer == null || playerInventory == null) return;

        if (playerInventory.RemoveItem(item, 1))
        {
            currentContainer.inventory.AddItem(item, 1);
            Refresh();
        }
    }

    public void TransferFromStorageToPlayer(ItemData item)
    {
        if (item == null || currentContainer == null || playerInventory == null) return;

        if (currentContainer.inventory.RemoveItem(item, 1))
        {
            playerInventory.AddItem(item, 1);
            Refresh();
        }
    }
}