using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public Button button;

    private ItemData item;
    private int amount;
    private StorageUI storageUI;
    private bool isPlayerSide;

    public ItemData Item => item;
    public int Amount => amount;

    public void Setup(ItemData item, int amount, StorageUI ui, bool isPlayerSide)
    {
        this.item = item;
        this.amount = amount;
        this.storageUI = ui;
        this.isPlayerSide = isPlayerSide;

        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = item.icon != null;
            icon.color = item.icon != null ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        if (amountText != null)
            amountText.text = amount.ToString();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    public void Clear()
    {
        item = null;
        amount = 0;

        if (icon != null) icon.enabled = false;
        if (amountText != null) amountText.text = "";
        if (button != null) button.onClick.RemoveAllListeners();
    }

    void OnClick()
    {
        if (item == null || storageUI == null) return;

        if (isPlayerSide)
            storageUI.TransferFromPlayerToStorage(item);
        else
            storageUI.TransferFromStorageToPlayer(item);
    }
}