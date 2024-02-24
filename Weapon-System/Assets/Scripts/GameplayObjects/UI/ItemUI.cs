using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class ItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        Image m_Icon;

        [SerializeField]
        TextMeshProUGUI m_NameText;

        [SerializeField]                    // Only for Debugging purpose
        ItemDataSO m_ItemData;
        public ItemDataSO ItemData => m_ItemData;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Vector2 m_lastAnchoredPosition;

        [HideInInspector]
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

        public void SetItemData(ItemDataSO itemData)
        {
            m_ItemData = itemData;
            m_Icon.sprite = itemData.IconSprite;
            m_NameText.text = itemData.name;
        }
    }
}
