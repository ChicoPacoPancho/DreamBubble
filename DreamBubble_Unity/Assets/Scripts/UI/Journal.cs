using System.Collections;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class Journal : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject m_ItemListPrefab;

    [SerializeField]
    private SelectableItem m_testItem;



    private RectTransform m_RectTransform;
    private float m_StartX;


    [SerializeField]
    private GameObject m_journalList;

    [SerializeField] private float m_RevealSpeed = 0.5f;
    [SerializeField] private float m_WriteDelay = 0.25f;
    [SerializeField] private float m_TimeOnScreen = 3f;
    [SerializeField] private float m_HideSpeed = 0.65f;

    private bool m_IsRevealed = false;


    [SerializeField] private GameObject m_EndText;
    
    
    private void Awake()
    {
        Globals.OnDreamItemsChanged += Globals_OnDreamItemsChanged;

        DayLevelController.OnDayCompleted += DayLevelController_OnDayCompleted;

        m_RectTransform = transform as RectTransform;
        m_StartX = m_RectTransform.anchoredPosition.x;
    }

    private void OnDestroy()
    {
        Globals.OnDreamItemsChanged -= Globals_OnDreamItemsChanged;
        DayLevelController.OnDayCompleted -= DayLevelController_OnDayCompleted;
    }

    private void DayLevelController_OnDayCompleted()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0.5f, m_RectTransform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.InSine));
        sequence.AppendCallback(() => { m_EndText.SetActive(true);});
        sequence.Append(m_RectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
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
        if(m_IsRevealed)
        {
            //skip animation and just add to the list if the journal is already revealed
            if (updateList) UpdateList();   

            return;
        }

        m_IsRevealed = true;

        UpdateList();
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(m_RectTransform.DOAnchorPosX(0, m_RevealSpeed).SetEase(Ease.InOutSine));
        sequence.Insert(m_TimeOnScreen, m_RectTransform.DOAnchorPosX(m_StartX, m_HideSpeed).SetEase(Ease.InOutSine));
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
