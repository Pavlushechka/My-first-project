using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject npcPrefab;
    public int count = 2;
    public float radius = 20f;

    [Header("Position")]
    public Transform spawnCenter;
    public float raycastHeight = 200f;
    public LayerMask groundMask = ~0;

    [Header("Roles")]
    public NPCRole defaultRole = NPCRole.Woodcutter;

    [Header("Debug")]
    public bool debugLogs = true;

    private static int nextNpcId = 1;

    void Start()
    {
        if (npcPrefab == null)
        {
            Debug.LogError("[NPCSpawner] NPC Prefab не назначен!");
            return;
        }

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = count * 30;

        while (spawned < count && attempts < maxAttempts)
        {
            attempts++;

            Vector3 center = spawnCenter != null ? spawnCenter.position : transform.position;
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(circle.x, 0f, circle.y);

            // Raycast на землю
            if (!Physics.Raycast(candidate + Vector3.up * raycastHeight, Vector3.down,
                out RaycastHit hit, raycastHeight * 2f, groundMask))
                continue;

            Vector3 spawnPos = hit.point + Vector3.up * 0.5f;

            // Проверяем, что точка на NavMesh
            if (!NavMesh.SamplePosition(spawnPos, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
                continue;

            Vector3 finalPos = navHit.position;

            // Спавн
            var obj = Instantiate(npcPrefab, finalPos, Quaternion.identity);
            obj.name = $"NPC_Peasant(Clone)_#{nextNpcId}";

            var brain = obj.GetComponent<NPCBrain>();
            if (brain != null)
            {
                brain.npcId = nextNpcId;
                brain.role = defaultRole;
            }

            if (debugLogs)
                Debug.Log($"[NPCSpawner] Спавн NPC #{nextNpcId} в {finalPos}");

            nextNpcId++;
            spawned++;
        }

        if (spawned < count)
            Debug.LogWarning($"[NPCSpawner] Заспавнено {spawned}/{count} (не хватило места на NavMesh)");
    }
}