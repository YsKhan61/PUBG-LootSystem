using UnityEngine;
using UnityEngine.EventSystems;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class ItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        ItemTagSO m_ItemTag;
        public ItemTagSO ItemTag => m_ItemTag;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Vector2 m_lastAnchoredPosition;

        public bool IsDragSuccess;

        private void Awake()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null)
            {
                Debug.LogError("No Canvas found in parent of " + gameObject.name);
                enabled = false;
                return;
            }

            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_lastAnchoredPosition = m_RectTransform.anchoredPosition;
            IsDragSuccess = false;

            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_RectTransform.anchoredPosition += eventData.delta * m_Canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.alpha = 1f;

            if (!IsDragSuccess)
            {
                m_RectTransform.anchoredPosition = m_lastAnchoredPosition;
            }
        }
    }

}
