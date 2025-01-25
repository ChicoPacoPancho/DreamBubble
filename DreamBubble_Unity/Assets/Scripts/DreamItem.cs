using UnityEngine;

public class DreamItem : MonoBehaviour
{
    [SerializeField]
    private  DreamItemData m_Data;
    public DreamItemData data => m_Data;

    private Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = true;
        m_Rigidbody.useGravity = false;
    }

    public void Interact()
    {
        Debug.Log("Interacted with " + m_Data.name);
        Globals.AddDreamItem(m_Data);
    }

    public void PlaceInDream()
    {
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

        transform.SetParent(null);
    }
}
