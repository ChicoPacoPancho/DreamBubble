using UnityEngine;

public class CharacterController_Dreamscape : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float airSpeedMultiplier = 0.5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float maxGroundAngle = 45f;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody rb;
    private bool isGrounded;
    private float horizontalInput;
    private Vector3 groundNormal = Vector3.up;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private const float JUMP_GRACE_PERIOD = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Freeze rotation to prevent tipping
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Flip character based on movement direction
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }

        // Handle jump timer
        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= JUMP_GRACE_PERIOD)
            {
                isJumping = false;
                jumpTimer = 0f;
            }
        }

        // Check if grounded using spherecast with a small forward offset
        RaycastHit hit;
        Vector3 rayStart = groundCheck.position + Vector3.up * groundCheckRadius;
        Vector3 rayDirection = Vector3.down;
        float rayDistance = groundCheckRadius * 2f;

        // Only check for ground if we're not in jump grace period
        if (!isJumping)
        {
            // Do three raycasts - one center, one slightly forward, one slightly back
            bool centerHit = Physics.SphereCast(rayStart, groundCheckRadius, rayDirection, out hit, rayDistance, groundLayer);
            bool forwardHit = Physics.SphereCast(rayStart + transform.forward * groundCheckDistance, groundCheckRadius, rayDirection, out hit, rayDistance, groundLayer);
            bool backHit = Physics.SphereCast(rayStart - transform.forward * groundCheckDistance, groundCheckRadius, rayDirection, out hit, rayDistance, groundLayer);
            
            isGrounded = centerHit || forwardHit || backHit;

            // Only consider surface ground if it's within our max angle
            if (isGrounded)
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                isGrounded = angle <= maxGroundAngle;
                groundNormal = isGrounded ? hit.normal : Vector3.up;
            }
            else
            {
                groundNormal = Vector3.up;
            }
        }

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            jumpTimer = 0f;
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }

    private void FixedUpdate()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        float targetSpeed = horizontalInput * moveSpeed;
        
        // Apply air speed multiplier when not grounded
        if (!isGrounded)
        {
            targetSpeed *= airSpeedMultiplier;
        }
        
        // Calculate acceleration rate based on whether we're trying to stop or move
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        
        // Calculate new horizontal velocity
        float speedDif = targetSpeed - currentVelocity.x;
        float movement = speedDif * accelRate * Time.fixedDeltaTime;
        
        // Calculate movement direction along the slope
        Vector3 moveDirection = Vector3.right;
        if (isGrounded)
        {
            // Project movement onto the slope and maintain speed
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
            
            // Apply downward force to keep character on slopes
            rb.AddForce(Vector3.down * 20f, ForceMode.Force);
            
            // Align velocity with slope to prevent bouncing
            Vector3 slopeVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, groundNormal);
            rb.linearVelocity = slopeVelocity;
        }
        
        // Apply horizontal force
        rb.AddForce(movement * moveDirection, ForceMode.VelocityChange);
        
        // Apply extra gravity when falling
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        
        // Clamp horizontal velocity
        Vector3 clampedVelocity = rb.linearVelocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -moveSpeed, moveSpeed);
        rb.linearVelocity = clampedVelocity;
    }

    // Optional: visualize ground check in editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
