using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    public float range = 3.5f;
    public LayerMask interactMask;
    public float sphereRadius = 0.2f;
    public float originHeight = 1.4f;

    [Header("Debug")]
    public bool debugLogs = false;

    public event System.Action<Interactable> OnTargetChanged;

    private PlayerInputActions input;
    private Interactable currentTarget;
    private Camera cam;
    public Interactable CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null && !currentTarget.Equals(null);

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Interact.performed += OnInteract;
    }

    void OnDisable()
    {
        input.Player.Interact.performed -= OnInteract;
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (currentTarget == null) return;

        Interactable target = currentTarget;
        target.Interact(gameObject);

        if (target == null || target.Equals(null) || !target.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            OnTargetChanged?.Invoke(null);
        }
    }

    void Update()
    {
        // Если цель стала "мёртвой" (объект уничтожен) — сбрасываем
        if (currentTarget != null && currentTarget.Equals(null))
        {
            currentTarget = null;
            OnTargetChanged?.Invoke(null);
        }

        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                if (debugLogs) Debug.LogWarning("PlayerInteractor: камера не найдена (нет тега MainCamera)");
                return;
            }
        }

        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;
        Ray ray = new Ray(origin, direction);

        Interactable newTarget = null;

        if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, range, interactMask, QueryTriggerInteraction.Collide))
        {
            if (debugLogs)
                Debug.Log($"SphereCast: {hit.collider.name} | layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)} | dist: {hit.distance:F2}");

            newTarget = hit.collider.GetComponentInParent<Interactable>();
        }
        else if (debugLogs)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit rawHit, range * 3f, ~0, QueryTriggerInteraction.Collide))
                Debug.Log($"SphereCast мимо. Raycast видит: {rawHit.collider.name} | layer: {LayerMask.LayerToName(rawHit.collider.gameObject.layer)} | dist: {rawHit.distance:F2}");
            else
                Debug.Log($"Ничего не найдено. Origin y={origin.y}, Dir={direction}");
        }

        if (newTarget != currentTarget)
        {
            currentTarget = newTarget;
            OnTargetChanged?.Invoke(currentTarget);
        }
    }
}