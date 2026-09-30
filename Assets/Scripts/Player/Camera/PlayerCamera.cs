using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public enum Mode { FirstPerson, ThirdPerson }

    [Header("Mode")]
    public Mode mode = Mode.FirstPerson;

    [Header("References")]
    public Transform playerBody;
    public Transform cameraPivot;

    [Header("Look")]
    public float mouseSensitivity = 0.12f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("First Person")]
    public Vector3 fpOffset = new Vector3(0f, 1.65f, 0f);

    [Header("Third Person")]
    public float tpDistance = 4.5f;
    public float tpMinDistance = 1.2f;
    public float tpHeight = 1.6f;
    public float tpSmooth = 12f;
    public float tpCollisionRadius = 0.3f;
    public LayerMask tpCollisionMask = ~0;

    private float yaw;
    private float pitch;
    private Vector2 lookInput;
    private Vector3 tpVelocity;
    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled  += OnLookCanceled;
        input.Player.ToggleCamera.performed += OnToggleCamera;
    }

    void OnDisable()
    {
        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled  -= OnLookCanceled;
        input.Player.ToggleCamera.performed -= OnToggleCamera;
    }

    void OnLook(InputAction.CallbackContext ctx)         => lookInput = ctx.ReadValue<Vector2>();
    void OnLookCanceled(InputAction.CallbackContext ctx) => lookInput = Vector2.zero;
    void OnToggleCamera(InputAction.CallbackContext ctx) =>
        mode = mode == Mode.FirstPerson ? Mode.ThirdPerson : Mode.FirstPerson;

   void LateUpdate()
   {
    // Если курсор разблокирован (открыто меню) — не вращаем камеру
    if (Cursor.lockState != CursorLockMode.Locked) return;

    yaw   += lookInput.x * mouseSensitivity;
    pitch -= lookInput.y * mouseSensitivity;
    pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

    Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

    if (mode == Mode.FirstPerson) ApplyFirstPerson(rot);
    else                          ApplyThirdPerson(rot);
    }

    void ApplyFirstPerson(Quaternion rot)
    {
        Vector3 basePos = cameraPivot != null ? cameraPivot.position : transform.position;
        transform.position = basePos + fpOffset;
        transform.rotation = rot;
    }

    void ApplyThirdPerson(Quaternion rot)
    {
        Vector3 pivot = (cameraPivot != null ? cameraPivot.position : transform.position) + Vector3.up * tpHeight;
        Vector3 desired = pivot - rot * Vector3.forward * tpDistance;

        if (Physics.SphereCast(pivot, tpCollisionRadius,
            (desired - pivot).normalized, out RaycastHit hit,
            tpDistance, tpCollisionMask, QueryTriggerInteraction.Ignore))
        {
            desired = pivot - rot * Vector3.forward * Mathf.Max(hit.distance - 0.1f, tpMinDistance);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref tpVelocity, 1f / tpSmooth);
        transform.rotation = rot;

        if (playerBody != null)
            playerBody.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}