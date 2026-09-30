using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Position")]
    [Tooltip("X, Z — координаты спавна. Y игнорируется, берётся из Terrain.")]
    public Vector2 spawnXZ = new Vector2(0f, 0f);

    [Header("Raycast")]
    public float raycastHeight = 200f;
    public LayerMask groundMask = ~0;
    public float spawnHeightOffset = 0.5f;

    [Header("References")]
    public Transform player;
    public CharacterController characterController;

    [Header("Debug")]
    public bool debugLogs = true;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("[PlayerSpawner] Player не назначен!");
            return;
        }

        Vector3 origin = new Vector3(spawnXZ.x, raycastHeight, spawnXZ.y);

        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundMask))
        {
            Debug.LogWarning("[PlayerSpawner] Raycast не попал в землю! Спавн по умолчанию.");
            return;
        }

        Vector3 spawnPos = hit.point + Vector3.up * spawnHeightOffset;

        // Отключаем CharacterController на момент перемещения
        if (characterController != null)
            characterController.enabled = false;

        player.position = spawnPos;

        if (characterController != null)
            characterController.enabled = true;

        if (debugLogs)
            Debug.Log($"[PlayerSpawner] Игрок заспавнен в {spawnPos}");
    }
}