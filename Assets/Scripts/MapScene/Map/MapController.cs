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
    private Vector2 m_scrollingArea;
    private Vector2 m_viewArea;

    private void Awake() {
        m_instance = this;
        m_contentSize = m_content.sizeDelta;
        m_viewArea = new Vector2(Screen.width * 0.8f, Screen.height * 0.9f);

        m_scrollingArea = m_contentSize - m_viewArea;
    }

    public void MoveToPosition(Vector2 localPosition) {
        float scrollPositionX = localPosition.x - m_viewArea.x / 2 + m_contentSize.x / 2;
        scrollPositionX = scrollPositionX < 0 ? 0 : scrollPositionX;
        scrollPositionX = scrollPositionX > m_scrollingArea.x ? m_scrollingArea.x : scrollPositionX;
        float scrollPositionY = localPosition.y - m_viewArea.y / 2 + m_contentSize.y / 2;
        scrollPositionY = scrollPositionY < 0 ? 0 : scrollPositionY;
        scrollPositionY = scrollPositionY > m_scrollingArea.y ? m_scrollingArea.y : scrollPositionY;

        Vector2 normalizedPosition = new Vector2(scrollPositionX / m_scrollingArea.x, scrollPositionY / m_scrollingArea.y);
        m_scrollRect.horizontalNormalizedPosition = normalizedPosition.x;
        m_scrollRect.verticalNormalizedPosition = normalizedPosition.y;
    }

}