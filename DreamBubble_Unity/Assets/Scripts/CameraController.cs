using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // The player to follow
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Default camera offset
    [SerializeField] private float smoothSpeed = 5f; // How smoothly the camera follows

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
