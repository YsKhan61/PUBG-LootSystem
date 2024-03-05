using TMPro;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class ItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, ISpawnable
    {
        [SerializeField]
        Image m_Icon;

        [SerializeField]
        TextMeshProUGUI m_NameText;

        [SerializeField]
        int m_PoolSize = 20;

        private InventoryUI m_InventoryUI;
        public InventoryUI InventoryUI => m_InventoryUI;

        private InventoryItem m_Item;
        public InventoryItem Item => m_Item;

        public string Name => gameObject.name;

        public GameObject GameObject => gameObject;

        public int PoolSize => m_PoolSize;

        public bool IsDragSuccess { get; private set; }

        /// <summary>
        /// The slot type of the slot where this ItemUI is currently stored
        /// </summary>
        public SlotType StoredSlotType { get; private set; }

        private RectTransform m_RectTransform;
        private CanvasGroup m_CanvasGroup;
        private Transform m_LastParent;
        private Vector2 m_lastAnchoredPosition;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();
        }

        // Need to implement later
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("Left click on itemUI " + Name);
                // Left click to use item
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("Right click on itemUI " + Name);
                // Right click to drop item
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_lastAnchoredPosition = m_RectTransform.anchoredPosition;
            m_LastParent = m_RectTransform.parent;

            IsDragSuccess = false;

            m_CanvasGroup.blocksRaycasts = false;
            // Set the parent to the canvas so that the UI is not clipped by the parent
            transform.SetParent(m_InventoryUI.CanvasTransform);
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

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
            {
                return;
            }

            if (!eventData.pointerDrag.TryGetComponent(out ItemUI itemUI))
            {
                return;
            }

            m_InventoryUI.OnItemUIDroppedOnSlotType(itemUI, StoredSlotType);
        }

        public void SetItemDataAndShow(InventoryItem item, InventoryUI inventoryUI, SlotType storedSlotType)
        {
            m_InventoryUI = inventoryUI;
            StoredSlotType = storedSlotType;
            m_Item = item;
            m_Icon.sprite = m_Item.ItemData.IconSprite;
            m_NameText.text = m_Item.ItemData.name;

            Show();
        }

        public void ResetItemDataAndHide()
        {
            m_Item = null;
            m_Icon.sprite = null;
            m_NameText.text = string.Empty;

            Hide();
        }

        public void FallbackToLastPosition()
        {
            m_RectTransform.anchoredPosition = m_lastAnchoredPosition;
            m_RectTransform.SetParent(m_LastParent);
        }

        public void OnDragSuccess(SlotType storedSlotType)
        {
            StoredSlotType = storedSlotType;
            IsDragSuccess = true;
            Show();
            BlockRaycast();
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
