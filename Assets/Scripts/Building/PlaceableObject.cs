using UnityEngine;

[CreateAssetMenu(fileName = "NewPlaceable", menuName = "Game/Placeable")]
public class PlaceableObject : ScriptableObject
{
    [Header("Identity")]
    public string buildingTypeId = "warehouse";
    public string displayName = "Постройка";
    public Sprite icon;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Grid")]
    [Tooltip("Размер постройки в клетках сетки (X, Y, Z)")]
    public Vector3Int gridSize = new Vector3Int(1, 1, 1);

    [Header("Cost")]
    public ItemData[] requiredItems;
    public int[] requiredAmounts;
}