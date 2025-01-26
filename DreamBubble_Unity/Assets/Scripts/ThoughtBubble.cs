using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using DG.Tweening;

public class ThoughtBubble : MonoBehaviour, IPointerClickHandler
{
    
    [SerializeField]
    private Transform m_TargetTransform;

    private Vector3 m_TargetOffset;

    public float speed = 10f;

    public int m_DreamItemId = 0;
    private DreamItem m_DreamItem;
    public DreamItemData testDreamItem;


    private void Start() 
    {
        Globals.AddDreamItem(testDreamItem);

        m_TargetOffset = transform.position - m_TargetTransform.position;

        DreamItemData itemData = Globals.dreamItems[m_DreamItemId];

        if(itemData == null)
        {
            Debug.LogError($"Dream Item Data for '{name}' is null");
            return;
        }

        Debug.Log("Dream Item: " + itemData.name);

        m_DreamItem = Instantiate(itemData.prefab, transform.position, Quaternion.identity, transform).GetComponent<DreamItem>();
    }

    public void UpdateFollowPosition()
    {
        Vector3 targetPosition = m_TargetTransform.position + m_TargetOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }

    private void Update()
    {
        UpdateFollowPosition();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on " + name);

        Pop();
    }

    public void Pop()
    {
        transform.DOScale(Vector3.one * 1.5f, 0.25f).SetEase(Ease.OutBack).OnComplete(() => {Destroy(gameObject);});

        

        m_DreamItem.PlaceInDream();
        
    }
}
