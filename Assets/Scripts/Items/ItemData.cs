using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId = "wood";
    public string displayName = "Древесина";

    [Header("Visual")]
    public Sprite icon;

    [Header("Stacking")]
    public int maxStack = 99;
}