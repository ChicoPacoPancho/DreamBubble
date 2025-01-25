using UnityEngine;

[CreateAssetMenu(fileName = "DreamItemData", menuName = "DreamItemData", order = 1)]
public class DreamItemData : ScriptableObject
{
    [SerializeField]
    private GameObject m_Prefab;
    public GameObject prefab => m_Prefab;


    [SerializeField]
    private string m_Name;
    new public string name => m_Name;
}
