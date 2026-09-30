using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int version = 1;
    public string saveTime;

    public PlayerSaveData player = new PlayerSaveData();
    public List<ItemStackSaveData> inventory = new List<ItemStackSaveData>();
    public List<BuildingSaveData> buildings = new List<BuildingSaveData>();
    public List<NPCSaveData> npcs = new List<NPCSaveData>();
    public List<DestroyedNodeSaveData> destroyedNodes = new List<DestroyedNodeSaveData>();
}

[Serializable]
public class PlayerSaveData
{
    public float posX;
    public float posY;
    public float posZ;
    public float rotY;

    public float health;
    public float hunger;
    public float thirst;

    public string equippedItemId;
}

[Serializable]
public class ItemStackSaveData
{
    public string itemId;
    public int amount;
}

[Serializable]
public class BuildingSaveData
{
    public string buildingTypeId;
    public float posX;
    public float posY;
    public float posZ;
    public float rotY;

    public List<ItemStackSaveData> storageItems = new List<ItemStackSaveData>();
}

[Serializable]
public class NPCSaveData
{
    public float posX;
    public float posY;
    public float posZ;

    public float hunger;
    public float energy;

    public string carriedItemId;
    public int carriedAmount;
}

[Serializable]
public class DestroyedNodeSaveData
{
    public string nodeTypeId;
    public float posX;
    public float posY;
    public float posZ;
}