using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnSettings
    {
        public GameObject prefab;
        public int count = 50;
        public float areaSize = 450f;
        public float minDistanceBetween = 4f;
        [Range(0f, 1f)] public float minHeightPercent = 0f;
        [Range(0f, 1f)] public float maxHeightPercent = 1f;
    }

    public SpawnSettings[] spawnGroups;
    public Transform spawnCenter;
    public LayerMask groundMask = ~0;
    public float raycastHeight = 200f;
    public float maxTerrainHeight = 80f;

    private List<Vector3> placedPositions = new List<Vector3>();

    IEnumerator Start()
    {
        // Ждём один кадр — меш успеет создаться
        yield return null;

        placedPositions.Clear();

        foreach (var group in spawnGroups)
        {
            if (group.prefab == null) continue;

            int attempts = 0;
            int placed = 0;
            int maxAttempts = group.count * 30;

            while (placed < group.count && attempts < maxAttempts)
            {
                attempts++;

                Vector3 center = spawnCenter != null ? spawnCenter.position : transform.position;
                float half = group.areaSize * 0.5f;
                Vector3 candidate = center + new Vector3(
                    Random.Range(-half, half),
                    0f,
                    Random.Range(-half, half)
                );

                if (!Physics.Raycast(candidate + Vector3.up * raycastHeight, Vector3.down,
                    out RaycastHit hit, raycastHeight * 2f, groundMask))
                    continue;

                Vector3 finalPos = hit.point;

                float heightPercent = finalPos.y / maxTerrainHeight;
                if (heightPercent < group.minHeightPercent || heightPercent > group.maxHeightPercent)
                    continue;

                bool tooClose = false;
                foreach (var pos in placedPositions)
                {
                    if (Vector3.Distance(finalPos, pos) < group.minDistanceBetween)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;

                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                Instantiate(group.prefab, finalPos, rot, transform);

                placedPositions.Add(finalPos);
                placed++;
            }

            Debug.Log($"Spawned {placed}/{group.count} of {group.prefab.name}");
        }
    }
}