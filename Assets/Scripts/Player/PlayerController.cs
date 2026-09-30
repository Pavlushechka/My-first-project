using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    [Header("Stamina")]
    public float runStaminaCost = 15f;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private PlayerStats stats;
    private PlayerInputActions input;

    private Vector2 moveInput;
    private bool runHeld;
    private bool jumpQueued;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
        input = new PlayerInputActions();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void OnEnable()
    {
        input.Player.Enable();

        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled  += OnMoveCanceled;
        input.Player.Run.performed  += OnRunPerformed;
        input.Player.Run.canceled   += OnRunCanceled;
        input.Player.Jump.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled  -= OnMoveCanceled;
        input.Player.Run.performed  -= OnRunPerformed;
        input.Player.Run.canceled   -= OnRunCanceled;
        input.Player.Jump.performed -= OnJumpPerformed;

        input.Player.Disable();
    }

    void OnMove(InputAction.CallbackContext ctx)         => moveInput = ctx.ReadValue<Vector2>();
    void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
    void OnRunPerformed(InputAction.CallbackContext ctx) => runHeld = true;
    void OnRunCanceled(InputAction.CallbackContext ctx)  => runHeld = false;
    void OnJumpPerformed(InputAction.CallbackContext ctx) => jumpQueued = true;

    void Update()
    {
        if (cameraTransform == null) return;

        Vector3 wishDir = GetWishDirection();

        if (!stats.IsDead)
            ApplyHorizontalMovement(wishDir);
        else
            horizontalVelocity = Vector3.zero;

        ApplyGravity();

        // ОДИН вызов Move за кадр
        Vector3 motion = horizontalVelocity * Time.deltaTime;
        motion.y = verticalVelocity * Time.deltaTime;
        controller.Move(motion);

    
    }

    Vector3 GetWishDirection()
    {
        Vector3 camForward = cameraTransform.forward; camForward.y = 0f; camForward.Normalize();
        Vector3 camRight   = cameraTransform.right;   camRight.y   = 0f; camRight.Normalize();

        Vector3 wishDir = camForward * moveInput.y + camRight * moveInput.x;
        if (wishDir.sqrMagnitude > 1f) wishDir.Normalize();
        return wishDir;
    }

    void ApplyHorizontalMovement(Vector3 wishDir)
    {
        bool wantsRun = runHeld && moveInput.sqrMagnitude > 0.01f && stats.currentStamina > 0f;
        float targetSpeed = wantsRun ? runSpeed : walkSpeed;

        if (wantsRun) stats.TryUseStamina(runStaminaCost * Time.deltaTime);

        Vector3 targetVelocity = wishDir * targetSpeed;

        float accel = wishDir.sqrMagnitude > 0.01f ? acceleration : deceleration;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, accel * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            // Небольшой прижим вниз, чтобы isGrounded стабильно возвращал true
            verticalVelocity = -2f;

            if (jumpQueued)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        jumpQueued = false;
        verticalVelocity += gravity * Time.deltaTime;

        // Ограничение падения, чтобы не проваливаться сквозь пол
        if (verticalVelocity < -50f) verticalVelocity = -50f;
    }
}