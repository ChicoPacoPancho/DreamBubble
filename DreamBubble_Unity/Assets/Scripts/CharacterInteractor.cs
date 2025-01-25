using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractor : MonoBehaviour
{

    private List<DreamItem> m_IntersectingItems = new List<DreamItem>();
    private DreamItem m_HighlightedItem;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent<DreamItem>(out DreamItem dreamItem))
        {
            m_IntersectingItems.Add(dreamItem);
        }

        CollidedItemsChanged();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<DreamItem>(out DreamItem dreamItem))
        {
            m_IntersectingItems.Remove(dreamItem);
        }

        CollidedItemsChanged();
    }

    private void CollidedItemsChanged()
    {
        if(GetClosestDreamItem(ref m_HighlightedItem))
        {
            //highlight item
        }
    }

    private bool GetClosestDreamItem(ref DreamItem closestItem)
    {
        float closestDistance = Mathf.Infinity;
        float itemDistance;
        bool changed = false;
        
        if(closestItem != null)
        {
            closestDistance = Vector3.Distance(transform.position, closestItem.transform.position);
        }

        foreach(DreamItem dreamItem in m_IntersectingItems)
        {
            itemDistance = Vector3.Distance(transform.position, dreamItem.transform.position);
            if(itemDistance < closestDistance)
            {
                closestDistance = itemDistance;
                if(closestItem != dreamItem)
                {
                    Debug.Log($"Closest Item Changed: {dreamItem.name}");
                    closestItem = dreamItem;
                    changed = true;
                }
            }
        }

        return changed;
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed");
            if(m_HighlightedItem != null)
            {
                Debug.Log($"Interacting with {m_HighlightedItem.name}");
                m_HighlightedItem.Interact();
            }
        }
    }
}
