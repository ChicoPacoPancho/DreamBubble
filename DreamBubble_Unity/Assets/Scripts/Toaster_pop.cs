using UnityEngine;
using System.Collections;  // Add this for coroutines

public class Toaster_pop : MonoBehaviour
{
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float activationDelay = 1f;
    [SerializeField] private float cooldownTime = 3f;
    [SerializeField] private ParticleSystem[] launchVFX;
    [SerializeField] private float launchDelay = 0.5f;  // Add this line - delay between VFX and launch

    private bool isCharacterInTrigger = false;
    private bool isOnCooldown = false;
    private float activationTimer = 0f;
    private CharacterController_Dreamscape playerController;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);
        
        // Try to get component on this object
        CharacterController_Dreamscape controller = other.GetComponent<CharacterController_Dreamscape>();
        
        // If not found, try to get from parent
        if (controller == null)
        {
            controller = other.GetComponentInParent<CharacterController_Dreamscape>();
            Debug.Log("Searching in parent for controller");
        }
        
        // If still not found, try to get from children
        if (controller == null)
        {
            controller = other.GetComponentInChildren<CharacterController_Dreamscape>();
            Debug.Log("Searching in children for controller");
        }

        if (controller != null)
        {
            Debug.Log("Found player controller on: " + controller.gameObject.name);
            isCharacterInTrigger = true;
            playerController = controller;
        }
        else
        {
            Debug.Log("Could not find CharacterController_Dreamscape on any related objects");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Try to get component on this object
        CharacterController_Dreamscape controller = other.GetComponent<CharacterController_Dreamscape>();
        
        // If not found, try to get from parent
        if (controller == null)
        {
            controller = other.GetComponentInParent<CharacterController_Dreamscape>();
        }
        
        // If still not found, try to get from children
        if (controller == null)
        {
            controller = other.GetComponentInChildren<CharacterController_Dreamscape>();
        }

        if (controller != null)
        {
            isCharacterInTrigger = false;
            activationTimer = 0f;
            playerController = null;
            Debug.Log("Player exited trigger - resetting toaster");
        }
    }

    private void Update()
    {
        if (isCharacterInTrigger && !isOnCooldown)
        {
            activationTimer += Time.deltaTime;
            
            if (activationTimer >= activationDelay)
            {
                LaunchPlayer();
                StartCooldown();
            }
        }
    }

    private void LaunchPlayer()
    {
        if (playerController != null)
        {
            Debug.Log("Attempting to launch player with force: " + launchForce);
            
            // Play all VFX immediately
            foreach (ParticleSystem vfx in launchVFX)
            {
                if (vfx != null)
                {
                    vfx.Play();
                }
            }
            
            // Start the delayed launch
            StartCoroutine(DelayedLaunch());
        }
        else
        {
            Debug.Log("Player controller is null!");
        }
    }

    private IEnumerator DelayedLaunch()
    {
        yield return new WaitForSeconds(launchDelay);
        
        if (playerController != null)  // Check again in case player left during delay
        {
            playerController.AddVerticalVelocity(launchForce);
        }
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        activationTimer = 0f;
        Invoke("ResetCooldown", cooldownTime);
    }

    private void ResetCooldown()
    {
        isOnCooldown = false;
    }
}
