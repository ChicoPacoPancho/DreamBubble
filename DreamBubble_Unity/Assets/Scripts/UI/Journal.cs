using System.Collections;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Journal : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject m_ItemListPrefab;

    [SerializeField]
    private SelectableItem m_testItem;


    [SerializeField]
    private GameObject m_journalList;

    [SerializeField] private float m_RevealSpeed = 0.5f;
    [SerializeField] private float m_WriteDelay = 0.25f;
    [SerializeField] private float m_TimeOnScreen = 3f;
    [SerializeField] private float m_HideSpeed = 0.65f;

    private bool m_IsRevealed = false;

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
        PlayRevealAnimation();
    }

    private void PlayRevealAnimation(bool updateList = true)
    {
        RectTransform rect = transform as RectTransform;
        float startX = rect.anchoredPosition.x;

        m_IsRevealed = true;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(rect.DOAnchorPosX(0, m_RevealSpeed).SetEase(Ease.InOutSine));
        sequence.Insert(m_TimeOnScreen, rect.DOAnchorPosX(startX, m_HideSpeed).SetEase(Ease.InOutSine));
        sequence.AppendCallback(() => m_IsRevealed = false);

        if (updateList)
        {
        sequence.InsertCallback(m_RevealSpeed + m_WriteDelay, UpdateList);
        }
    }

    private void UpdateList()
    {
        
        while(m_journalList.transform.childCount > 0)
        {
            DestroyImmediate(m_journalList.transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < Globals.dreamItems.Count; i++)
        {

            DreamItemData item = Globals.dreamItems[i];

            GameObject itemList = Instantiate(m_ItemListPrefab, m_journalList.transform);
            JournalListItem itemListComponent = itemList.GetComponent<JournalListItem>();
            itemListComponent.SetText($"{(i+1)}. {item.journalPhrase}");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_IsRevealed)
            return;

        PlayRevealAnimation(true);
    }
}
