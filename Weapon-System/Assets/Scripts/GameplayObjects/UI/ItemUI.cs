using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class ItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, ISpawnable
    {
        [SerializeField]
        Image m_Icon;

        [SerializeField]
        TextMeshProUGUI m_NameText;

        [SerializeField]
        int m_PoolSize = 20;

        private RectTransform m_RectTransform;
        // private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Vector2 m_lastAnchoredPosition;

        private InventoryUI m_InventoryUI;

        private InventoryItem m_Item;
        public InventoryItem Item => m_Item;

        public string Name => gameObject.name;

        public GameObject GameObject => gameObject;

        public int PoolSize => m_PoolSize;

        [HideInInspector]
        public bool IsDragSuccess;

        private void Awake()
        {
            /*m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null)
            {
                Debug.LogError("No Canvas found in parent of " + gameObject.name);
                enabled = false;
                return;
            }*/

            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                m_InventoryUI.RaiseOnInventoryItemUIRemovedEvent(m_Item);
                Destroy(gameObject);
            }
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
            m_RectTransform.anchoredPosition += eventData.delta * m_InventoryUI.CanvasScaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragSuccess)
            {
                m_CanvasGroup.blocksRaycasts = true;
                m_CanvasGroup.alpha = 1f;

                FallbackToLastPosition();
            }
        }

        public void SetItemDataAndShow(InventoryItem item, InventoryUI inventoryUI)
        {
            m_InventoryUI = inventoryUI;
            m_Item = item;
            m_Icon.sprite = m_Item.ItemData.IconSprite;
            m_NameText.text = m_Item.ItemData.name;

            Show();
            BlockRaycast();
        }

        public void ResetItemDataAndHide()
        {
            m_Item = null;
            m_Icon.sprite = null;
            m_NameText.text = string.Empty;

            Hide();
            UnblockRaycast();
        }

        public void FallbackToLastPosition()
        {
            m_RectTransform.anchoredPosition = m_lastAnchoredPosition;
        }

        public void Show()
        {
            m_CanvasGroup.alpha = 1f;
        }

        public void BlockRaycast()
        {
            m_CanvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            m_CanvasGroup.alpha = 0f;
        }

        public void UnblockRaycast()
        {
            m_CanvasGroup.blocksRaycasts = false;
        }
    }
}
