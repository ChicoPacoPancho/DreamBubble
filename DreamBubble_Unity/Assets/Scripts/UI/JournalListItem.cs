using UnityEngine;

public class JournalListItem : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_ItemText;

    public void SetText(string text)
    {
        m_ItemText.text = text;
    }
}
