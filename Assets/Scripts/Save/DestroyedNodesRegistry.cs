using System.Collections.Generic;
using UnityEngine;

public static class DestroyedNodesRegistry
{
    public static List<DestroyedNodeSaveData> destroyed = new List<DestroyedNodeSaveData>();

    public static void Add(string nodeTypeId, Vector3 position)
    {
        destroyed.Add(new DestroyedNodeSaveData
        {
            nodeTypeId = nodeTypeId,
            posX = Mathf.Round(position.x * 10f) / 10f,
            posY = Mathf.Round(position.y * 10f) / 10f,
            posZ = Mathf.Round(position.z * 10f) / 10f
        });
    }

    public static void Clear()
    {
        destroyed.Clear();
    }

    public static void ReplaceWith(List<DestroyedNodeSaveData> data)
    {
        destroyed.Clear();
        if (data != null)
            destroyed.AddRange(data);
    }
}