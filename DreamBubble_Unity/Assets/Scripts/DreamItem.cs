using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DreamItem : MonoBehaviour
{

    private Rigidbody m_Rigidbody;

    [SerializeField] private float m_BubbleScale = 1.0f;
    [SerializeField] private float m_DreamScale = 1.0f;
    private Collider[] m_Colliders;

    private void Awake()
    {
        transform.localScale = Vector3.one * m_BubbleScale;

        if(TryGetComponent<Rigidbody>(out m_Rigidbody))
        {
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
        }

        m_Colliders = GetComponentsInChildren<Collider>(true);
    }

    public void SetCollision(bool enabled)
    {
        foreach(Collider collider in m_Colliders)
        {
            collider.enabled = enabled;
        }
    }

    public void PlaceInDream()
    {

        SetCollision(true);

        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

        transform.SetParent(null);

        transform.DOScale(Vector3.one * m_DreamScale, 1.0f);
    }
}
