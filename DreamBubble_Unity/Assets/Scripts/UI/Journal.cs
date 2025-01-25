using UnityEngine;

public class Journal : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ItemListPrefab;

    [SerializeField]
    private DreamItem m_testItem;


    [SerializeField]
    private GameObject m_journalList;

    private void Awake()
    {
        Globals.OnDreamItemsChanged += Globals_OnDreamItemsChanged;
    }

    private void Update() 
    {
        if(m_testItem == null)
            return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            Globals.AddDreamItem(m_testItem.data);
        }
    }

    private void Globals_OnDreamItemsChanged()
    {
        UpdateList();
    }

    private void UpdateList()
    {
        for (int i = 0; i < Globals.dreamItems.Count; i++)
        {
            DreamItemData item = Globals.dreamItems[i];

            GameObject itemList = Instantiate(m_ItemListPrefab, m_journalList.transform);
            JournalListItem itemListComponent = itemList.GetComponent<JournalListItem>();
            itemListComponent.SetText($"{(i+1)}. {item.journalPhrase}");
        }
    }
}
