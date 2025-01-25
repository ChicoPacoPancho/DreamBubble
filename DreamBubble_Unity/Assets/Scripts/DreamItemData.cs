using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "DreamItemData", menuName = "DreamItemData", order = 1)]
public class DreamItemData : ScriptableObject
{

    [SerializeField]
    private string m_JournalPhrase;
    public string journalPhrase => m_JournalPhrase;

    [SerializeField]
    private GameObject m_Prefab;
    public GameObject prefab => m_Prefab;
}
