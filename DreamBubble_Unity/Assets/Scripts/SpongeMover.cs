using UnityEngine;

public class SpongMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Speed at which the object moves
    private bool isInWater = false;

    private void Update()
    {
        if (isInWater)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
        }
    }
}
