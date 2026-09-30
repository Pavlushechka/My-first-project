using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EquipmentSlot))]
[RequireComponent(typeof(Inventory))]
public class PlayerEquipmentController : MonoBehaviour
{
    [Header("Items")]
    public ItemData axeItem;
    public ItemData pickaxeItem;

    private EquipmentSlot equipment;
    private Inventory inventory;
    private PlayerInputActions input;

    void Awake()
    {
        equipment = GetComponent<EquipmentSlot>();
        inventory = GetComponent<Inventory>();
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.EquipSlot1.performed += OnEquip1;
        input.Player.EquipSlot2.performed += OnEquip2;
    }

    void OnDisable()
    {
        input.Player.EquipSlot1.performed -= OnEquip1;
        input.Player.EquipSlot2.performed -= OnEquip2;
    }

    void OnEquip1(InputAction.CallbackContext ctx)
    {
        TryEquip(axeItem, "Топор");
    }

    void OnEquip2(InputAction.CallbackContext ctx)
    {
        TryEquip(pickaxeItem, "Кирка");
    }

    void TryEquip(ItemData item, string itemName)
    {
        if (item == null) return;

        // Если предмет уже экипирован — разрешаем снять (даже если его нет в инвентаре)
        if (equipment.IsEquipped(item))
        {
            equipment.ToggleEquip(item);
            return;
        }

        // Иначе — проверяем, что предмет есть в инвентаре
        if (inventory.GetAmount(item) <= 0)
        {
            Debug.Log($"{itemName} нет в инвентаре — сначала скрафти");
            return;
        }

        equipment.ToggleEquip(item);
    }
}