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
    [SerializeField] private string platformTag = "Platform";
    [SerializeField] private string bounceTag = "Bounce";

    private AudioSource jumpAudio;
    private Rigidbody rb;
    private bool isGrounded;
    private float horizontalInput;
    private Vector3 groundNormal = Vector3.up;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private const float JUMP_GRACE_PERIOD = 0.1f;
    private Transform platformParent;
    private Vector3 lastPlatformPosition;
    private Vector3 platformVelocity;
    private Vector3 lastCharacterPosition;

    private PhysicsProperties currentSurface;

    [System.Serializable]
    private struct PhysicsProperties
    {
        public float friction;
        public float bounce;

        public PhysicsProperties(PhysicsMaterial material)
        {
            friction = material ? material.dynamicFriction : 1f;
            bounce = material ? material.bounciness : 0f;
        }
    }

    private void Awake()
    {
        jumpAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        // Freeze rotation to prevent tipping
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        // Store platform position at start of frame
        if (platformParent != null)
        {
            Vector3 platformDelta = platformParent.position - lastPlatformPosition;
            if (isGrounded)
            {
                // Move with platform
                transform.position += platformDelta;
            }
            lastPlatformPosition = platformParent.position;
        }

        // Get input and handle character flipping
        horizontalInput = Input.GetAxisRaw("Horizontal");

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

        if (!isJumping)
        {
            bool centerHit = Physics.SphereCast(rayStart, groundCheckRadius, rayDirection, out hit, rayDistance, groundLayer);
            bool forwardHit = Physics.SphereCast(rayStart + transform.forward * groundCheckDistance, groundCheckRadius, rayDirection, out hit, rayDistance, groundLayer);
            bool backHit = Physics.SphereCast(rayStart - transform.forward * groundCheckDistance, groundCheckRadius, rayDirection, out hit, rayDistance, groundLayer);
            
            isGrounded = centerHit || forwardHit || backHit;

            if (isGrounded && hit.collider != null)
            {
                // Check if this is a bounce surface
                if (hit.collider.CompareTag(bounceTag))
                {
                    // Allow bounce physics to take over
                    isGrounded = false;
                    groundNormal = Vector3.up;
                    
                    // Get physics material properties for bounce
                    PhysicsMaterial material = hit.collider.sharedMaterial;
                    if (material != null)
                    {
                        // Preserve downward velocity for proper bounce
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0);
                    }
                }
                else if (hit.collider.CompareTag(platformTag))
                {
                    // Normal platform handling
                    if (platformParent != hit.collider.transform)
                    {
                        platformParent = hit.collider.transform;
                        lastPlatformPosition = platformParent.position;
                    }
                }

                // Only consider surface ground if it's within our max angle and not a bounce surface
                if (!hit.collider.CompareTag(bounceTag))
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    isGrounded = angle <= maxGroundAngle;
                    groundNormal = isGrounded ? hit.normal : Vector3.up;
                    
                    // Get physics material properties
                    PhysicsMaterial material = hit.collider.sharedMaterial;
                    currentSurface = new PhysicsProperties(material);
                }
            }
            else
            {
                platformParent = null;
                groundNormal = Vector3.up;
                currentSurface = new PhysicsProperties(null);
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
            
            jumpAudio.Play();
        }
    }

    private void FixedUpdate()
    {
        // Remove platform movement from FixedUpdate
        
        // Regular movement code
        Vector3 currentVelocity = rb.linearVelocity;
        float targetSpeed = horizontalInput * moveSpeed;
        Vector3 position = transform.position;
        position.z = 0;
        transform.position = position;

        // Apply air speed multiplier when not grounded
        if (!isGrounded)
        {
            targetSpeed *= airSpeedMultiplier;
        }
        
        // Calculate acceleration rate based on whether we're trying to stop or move
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        
        // Apply surface friction to acceleration
        accelRate *= Mathf.Lerp(1f, currentSurface.friction, isGrounded ? 1f : 0f);
        
        // Calculate new horizontal velocity
        float speedDif = targetSpeed - currentVelocity.x;
        float movement = speedDif * accelRate * Time.fixedDeltaTime;
        
        // Calculate movement direction along the slope
        Vector3 moveDirection = Vector3.right;
        if (isGrounded && !isJumping)
        {
            // Project movement onto the slope and maintain speed
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
            
            // Apply downward force to keep character on slopes
            rb.AddForce(Vector3.down * 20f, ForceMode.Force);
            
            // Align velocity with slope to prevent bouncing
            Vector3 slopeVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, groundNormal);
            
            // Apply surface friction to velocity
            if (Mathf.Abs(horizontalInput) < 0.01f)
            {
                slopeVelocity *= (1f - currentSurface.friction * Time.fixedDeltaTime);
            }
            
            rb.linearVelocity = slopeVelocity;
        }
        
        // Apply horizontal force
        rb.AddForce(movement * moveDirection, ForceMode.VelocityChange);
        
        // Apply extra gravity when falling
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        
        // Clamp horizontal velocity (don't clamp on low friction surfaces)
        if (currentSurface.friction > 0.1f)
        {
            Vector3 clampedVelocity = rb.linearVelocity;
            clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -moveSpeed, moveSpeed);
            rb.linearVelocity = clampedVelocity;
        }
    }

    public void AddVerticalVelocity(float amount)
    {
        // Add upward force using the rigidbody
        rb.AddForce(Vector3.up * amount, ForceMode.VelocityChange);

        // Set jumping state to prevent immediate ground detection
        isJumping = true;
        jumpTimer = 0f;
        isGrounded = false;
        groundNormal = Vector3.up;
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

    private void OnDisable()
    {
        platformParent = null;
        platformVelocity = Vector3.zero;
    }
}
