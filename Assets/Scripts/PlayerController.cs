using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSControllerWithStates : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping
    }

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheckOrigin;

    private Rigidbody rb;
    private Animator animator;

    private Vector3 moveInput;
    private bool isGrounded;
    private PlayerState currentState = PlayerState.Idle;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        rb.freezeRotation = true;

        if (groundCheckOrigin == null)
            groundCheckOrigin = transform;
    }

    void Update()
    {
        // -------- Input --------
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveInput = (transform.right * x + transform.forward * z);
        moveInput = Vector3.ClampMagnitude(moveInput, 1f);

        // -------- Ground Check --------
        isGrounded = Physics.Raycast(
            groundCheckOrigin.position,
            Vector3.down,
            groundCheckDistance,
            groundMask
        );

        // -------- Jump Input --------
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            currentState = PlayerState.Jumping;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }

        // -------- Determine State --------
        if (currentState != PlayerState.Jumping) // don't override Jumping mid-air
        {
            if (moveInput.magnitude == 0)
            {
                currentState = PlayerState.Idle;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                currentState = PlayerState.Running;
            }
            else
            {
                currentState = PlayerState.Walking;
            }
        }

        // -------- Update Animator --------
        float speedPercent = moveInput.magnitude * ((currentState == PlayerState.Running) ? 1f : 0.5f);
        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("isRunning", currentState == PlayerState.Running);
        animator.SetBool("isWalking", currentState == PlayerState.Walking);
    }

    void FixedUpdate()
    {
        float targetSpeed = currentState switch
        {
            PlayerState.Running => runSpeed,
            PlayerState.Walking => walkSpeed,
            _ => 0f
        };

        Vector3 targetVelocity = moveInput * targetSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // Automatically return to Idle if grounded after Jump
        if (currentState == PlayerState.Jumping && isGrounded)
        {
            currentState = PlayerState.Idle;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckOrigin == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            groundCheckOrigin.position,
            groundCheckOrigin.position + Vector3.down * groundCheckDistance
        );
    }
}
