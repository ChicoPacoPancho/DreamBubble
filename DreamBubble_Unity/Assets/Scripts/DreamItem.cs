using UnityEngine;

public class DreamItem : MonoBehaviour
{

    private Rigidbody m_Rigidbody;

    private void Start()
    {
        if(TryGetComponent<Rigidbody>(out m_Rigidbody))
        {
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
        }
    }

    public void PlaceInDream()
    {
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

        transform.SetParent(null);
    }
}
