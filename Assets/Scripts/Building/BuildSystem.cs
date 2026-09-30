using UnityEngine;
using UnityEngine.InputSystem;

public class BuildSystem : MonoBehaviour
{
    [Header("Settings")]
    public float gridSize = 1f;
    public float maxBuildDistance = 10f;
    public LayerMask groundMask = ~0;

    [Header("Blocking Layers")]
    public LayerMask blockingMask;

    [Header("Debug")]
    public bool debugLogs = true;

    private PlaceableObject currentPlaceable;
    private GameObject currentGhost;
    private BuildGhost ghostScript;
    private PlayerInputActions input;
    private Camera cam;
    private bool currentPlacementValid;

    public bool IsBuilding => currentPlaceable != null;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.BuildPlace.performed += OnPlace;
        input.Player.BuildCancel.performed += OnCancel;
    }

    void OnDisable()
    {
        input.Player.BuildPlace.performed -= OnPlace;
        input.Player.BuildCancel.performed -= OnCancel;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!IsBuilding) return;
        if (cam == null) { cam = Camera.main; if (cam == null) return; }

        UpdateGhost();
    }

    public void StartPlacing(PlaceableObject placeable)
    {
        if (placeable == null || placeable.prefab == null)
        {
            if (debugLogs) Debug.LogWarning("PlaceableObject или его prefab = null");
            return;
        }

        CancelPlacing();

        currentPlaceable = placeable;
        currentGhost = Instantiate(placeable.prefab);
        currentGhost.name = "Ghost_" + placeable.displayName;

        ghostScript = currentGhost.AddComponent<BuildGhost>();
        ghostScript.SetValid(true);

        if (debugLogs) Debug.Log($"Начато размещение: {placeable.displayName}");
    }

    public void CancelPlacing()
    {
        if (currentGhost != null)
            Destroy(currentGhost);

        currentGhost = null;
        ghostScript = null;
        currentPlaceable = null;

        if (debugLogs) Debug.Log("Размещение отменено");
    }

    void UpdateGhost()
    {
        if (currentGhost == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 snappedPos = SnapToGrid(hit.point);
            currentGhost.transform.position = snappedPos;
            currentGhost.transform.rotation = Quaternion.identity;

            currentPlacementValid = CheckPlacement(snappedPos);

            if (ghostScript != null)
                ghostScript.SetValid(currentPlacementValid);
        }
        else
        {
            currentPlacementValid = false;
            if (ghostScript != null)
                ghostScript.SetValid(false);
        }
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        float x = Mathf.Round(pos.x / gridSize) * gridSize;
        float z = Mathf.Round(pos.z / gridSize) * gridSize;
        float y = pos.y;   // ← СОХРАНЯЕМ ВЫСОТУ ИЗ RAYCAST
        return new Vector3(x, y, z);
    }

    bool CheckPlacement(Vector3 position)
    {
        if (currentPlaceable == null) return false;

        Vector3Int grid = currentPlaceable.gridSize;

        Vector3 center = position + new Vector3(0f, grid.y * 0.5f, 0f);
        Vector3 halfExtents = new Vector3(grid.x * 0.5f, grid.y * 0.5f, grid.z * 0.5f);
        halfExtents *= 0.95f;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, blockingMask, QueryTriggerInteraction.Ignore);

        if (hits.Length > 0)
        {
            if (debugLogs)
                Debug.Log($"[BuildBlock] Место занято: {hits[0].name}");
            return false;
        }

        return true;
    }

    void OnPlace(InputAction.CallbackContext ctx)
    {
        if (!IsBuilding) return;
        if (currentGhost == null) return;

        if (!currentPlacementValid)
        {
            if (debugLogs) Debug.Log("Нельзя поставить — место занято");
            return;
        }

        var inv = GetComponent<Inventory>();
        if (inv != null)
        {
            if (!inv.HasItems(currentPlaceable.requiredItems, currentPlaceable.requiredAmounts))
            {
                if (debugLogs) Debug.Log($"Не хватает ресурсов для {currentPlaceable.displayName}");
                return;
            }

            inv.RemoveItems(currentPlaceable.requiredItems, currentPlaceable.requiredAmounts);
        }

        Vector3 pos = currentGhost.transform.position;
        Quaternion rot = currentGhost.transform.rotation;

        Instantiate(currentPlaceable.prefab, pos, rot);

        if (debugLogs) Debug.Log($"Поставлено: {currentPlaceable.displayName} в {pos}");
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        if (IsBuilding)
            CancelPlacing();
    }
}