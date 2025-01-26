using UnityEngine;

public class SinkingPlatform : MonoBehaviour
{
    [SerializeField] private float sinkSpeed = 2f;
    [SerializeField] private float sinkDelay = 0.5f;  // Time before platform starts sinking
    [SerializeField] private float maxSinkDistance = 10f;  // Maximum distance platform can sink
    [SerializeField] private float resetTime = 3f;  // Time to reset after player leaves

    private bool isPlayerOn = false;
    private float sinkTimer = 0f;
    private Vector3 startPosition;
    private float currentSinkDistance = 0f;
    private CharacterController_Dreamscape playerController;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Try to get component on this object
        CharacterController_Dreamscape controller = other.GetComponent<CharacterController_Dreamscape>();
        
        // If not found, try to get from parent
        if (controller == null)
        {
            controller = other.GetComponentInParent<CharacterController_Dreamscape>();
        }

        if (controller != null)
        {
            isPlayerOn = true;
            playerController = controller;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController_Dreamscape controller = other.GetComponent<CharacterController_Dreamscape>();
        if (controller == null)
        {
            controller = other.GetComponentInParent<CharacterController_Dreamscape>();
        }

        if (controller != null)
        {
            isPlayerOn = false;
            playerController = null;
            Invoke("ResetPlatform", resetTime);
        }
    }

    private void Update()
    {
        if (isPlayerOn)
        {
            sinkTimer += Time.deltaTime;
            
            if (sinkTimer >= sinkDelay && currentSinkDistance < maxSinkDistance)
            {
                // Calculate sink amount for this frame
                float sinkAmount = sinkSpeed * Time.deltaTime;
                currentSinkDistance += sinkAmount;
                
                Vector3 down = transform.InverseTransformDirection(Vector3.down);
                // Move platform down
                transform.Translate(down * sinkAmount);
                
            }
        }
    }

    private void ResetPlatform()
    {
        // Only reset if player is not on the platform
        if (!isPlayerOn)
        {
            transform.position = startPosition;
            currentSinkDistance = 0f;
            sinkTimer = 0f;
        }
    }
}
