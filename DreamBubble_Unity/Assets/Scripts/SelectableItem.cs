using UnityEngine;

public class SelectableItem : MonoBehaviour
{
    [SerializeField]
    private  DreamItemData m_Data;
    public DreamItemData data => m_Data;

    public void Interact()
    {
        Debug.Log("Interacted with " + m_Data.name);
        Globals.AddDreamItem(m_Data);
    }
}
