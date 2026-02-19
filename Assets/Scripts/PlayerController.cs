using UnityEngine;
using Photon.Pun;

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
    
    [Header("Player Resource system")]
    public PlayerResourceSystem resourceSystem;

    PhotonView PV;
    PlayerManager playerManager;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

        animator = GetComponentInChildren<Animator>();
        rb.freezeRotation = true;

        if (groundCheckOrigin == null)
            groundCheckOrigin = transform;
    }

    void Update()
    {
        if(!PV.IsMine) return;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveInput = (transform.right * x + transform.forward * z);
        moveInput = Vector3.ClampMagnitude(moveInput, 1f);
        
        isGrounded = Physics.Raycast(
            groundCheckOrigin.position,
            Vector3.down,
            groundCheckDistance,
            groundMask
        );
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            currentState = PlayerState.Jumping;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }

        if (currentState != PlayerState.Jumping)
        {
            if (moveInput.magnitude == 0)
            {
                currentState = PlayerState.Idle;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && resourceSystem.currentStamina > 0)
            {
                currentState = PlayerState.Running;
            }
            else
            {
                currentState = PlayerState.Walking;
            }
        }
        
        float speedPercent = moveInput.magnitude * ((currentState == PlayerState.Running) ? 1f : 0.5f);
        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("isRunning", currentState == PlayerState.Running);
        animator.SetBool("isWalking", currentState == PlayerState.Walking);
        HandleStamina();
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

    void HandleStamina()
    {
        if (resourceSystem == null)
        {
            return;
        }

        // Drain stamina while running and moving
        if (currentState == PlayerState.Running && moveInput.magnitude > 0)
        {
            int drain = Mathf.CeilToInt(15f * Time.deltaTime);
            resourceSystem.TakeStamina(drain);

            resourceSystem.staminaRegenTimer = resourceSystem.staminaCooldown;

            if (resourceSystem.currentStamina <= 0)
            {
                currentState = PlayerState.Walking;
            }
        }
        else
        {
            if (resourceSystem.staminaRegenTimer > 0f)
            {
                resourceSystem.staminaRegenTimer -= Time.deltaTime;
                return;
            }
            
            int regen = Mathf.CeilToInt(10f * Time.deltaTime);
            resourceSystem.currentStamina = Mathf.Clamp(
                resourceSystem.currentStamina + regen,
                0,
                resourceSystem.maxStamina
            );

            resourceSystem.staminaBar.SetValue(resourceSystem.currentStamina);
            resourceSystem.staminaBar.SetText(
                $"{resourceSystem.currentStamina} / {resourceSystem.maxStamina}"
            );
        }
    }


}
