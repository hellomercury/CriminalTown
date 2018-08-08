using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour {
    private static MapController m_instance;

    public static MapController Instance {
        get {
            return m_instance;
        }
    }
    
    [SerializeField]
    private ScrollRect m_scrollRect;
    [SerializeField]
    private RectTransform m_content;

    private Vector2 m_contentSize;
    
    private void Awake() {
        m_instance = this;
        m_contentSize = m_content.sizeDelta;
    }

    public void MoveToPosition(Vector2 localPosition) {
        Vector2 normalizedPositionFromCenter = localPosition / m_contentSize;
        m_scrollRect.horizontalNormalizedPosition = normalizedPositionFromCenter.x + 0.5f;
        m_scrollRect.verticalNormalizedPosition = normalizedPositionFromCenter.y + 0.5f;
    }
}
