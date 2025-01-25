using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractor : MonoBehaviour
{

    private List<SelectableItem> m_IntersectingItems = new List<SelectableItem>();
    private SelectableItem m_HighlightedItem;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent<SelectableItem>(out SelectableItem dreamItem))
        {
            m_IntersectingItems.Add(dreamItem);
        }
        

        CollidedItemsChanged();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<SelectableItem>(out SelectableItem dreamItem))
        {
            m_IntersectingItems.Remove(dreamItem);
        }

        CollidedItemsChanged();
    }

    private void CollidedItemsChanged()
    {
        // Gets the previous highlighted item before it potentially changes
        SelectableItem previousHighlighted = m_HighlightedItem;

        // Update the highlighted item
        if(GetClosestDreamItem(ref m_HighlightedItem))
        {
            // Turns off outline on previous item if it exists
            if (previousHighlighted != null && previousHighlighted.TryGetComponent<Outline>(out Outline oldOutline))
            {
                oldOutline.enabled = false;
            }

            // Turns on outline for the new highlighted item
            if (m_HighlightedItem != null && m_HighlightedItem.TryGetComponent<Outline>(out Outline newOutline))
            {
                newOutline.enabled = true;
            }
        }
        else if (m_HighlightedItem == null && previousHighlighted != null)
        {
            // Turns off outline when there is no highlighted item
            if (previousHighlighted.TryGetComponent<Outline>(out Outline outline))
            {
                outline.enabled = false;
            }
        }
    }

    private bool GetClosestDreamItem(ref SelectableItem closestItem)
    {
        float closestDistance = Mathf.Infinity;
        float itemDistance;
        bool changed = false;
        
        if(closestItem != null)
        {
            closestDistance = Vector3.Distance(transform.position, closestItem.transform.position);
        }

        if(m_IntersectingItems.Count == 0)
        {
            closestItem = null;
            return false;
        }

        foreach(SelectableItem dreamItem in m_IntersectingItems)
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
