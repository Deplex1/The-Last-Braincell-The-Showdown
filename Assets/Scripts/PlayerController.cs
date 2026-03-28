using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 7.5f;
    public float jumpForce = 16f;
    public float airControl = 0.65f;
    public float gravity = -35f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.4f;     // Increased to make it easier
    public LayerMask groundLayer;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void OnEnable()
    {
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
        crouchAction = new InputAction("Crouch", InputActionType.Button, "<Keyboard>/s");

        moveAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();

        jumpAction.performed += OnJump;
    }

    private void OnDisable()
    {
        jumpAction.performed -= OnJump;

        moveAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (!isGrounded)
        {
            float currentY = rb.velocity.y;
            rb.velocity = new Vector2(rb.velocity.x, currentY + gravity * Time.deltaTime);
        }

        float targetSpeed = moveInput.x * walkSpeed;

        if (isGrounded)
        {
            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }
        else
        {
            float newXSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, airControl);
            rb.velocity = new Vector2(newXSpeed, rb.velocity.y);
        }

        if (crouchAction.IsPressed() && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, -walkSpeed * 0.8f);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}