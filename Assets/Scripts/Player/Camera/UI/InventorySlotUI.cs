using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;

    public void Setup(ItemData item, int amount)
    {
        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = item.icon != null;
            icon.color = item.icon != null ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        if (amountText != null)
            amountText.text = amount > 1 ? amount.ToString() : "";
    }

    public void Clear()
    {
        if (icon != null) icon.enabled = false;
        if (amountText != null) amountText.text = "";
    }
}