using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class SightItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField]
        ItemUIType m_ItemUIType;
        public ItemUIType ItemUIType => m_ItemUIType;

        [SerializeField]
        Image m_Icon;

        public ItemDataSO ItemData { get; private set; }
        public int SlotIndex { get; private set; }

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Vector2 m_lastAnchoredPosition;

        [SerializeField]
        private WeaponInventoryUI m_InventoryUI;

        private ISightAttachment m_SightItem;
        public ISightAttachment Item => m_SightItem;

        /*private GunItem m_Item;
        public GunItem Item => m_Item;*/

        [HideInInspector]
        public bool IsDragSuccess;

        private ItemUI m_ItemUI;

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

            ///<remarks>
            /// Hide the item by default in awake,
            /// as WeaponInventoryUI's ToggleInventoryUI(false) is called in Start
            /// So if Hide() is called in start,
            /// It will call Hide() after Show() in SetItemData
            /// </remarks>
            Hide();     
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Store back the SightItem in the inventory by invoking an event
                // Make sure to reset the ItemUI's position to the last anchored position
                ResetItemDataAndHide();
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

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            /*if (!eventData.pointerDrag.TryGetComponent(out WeaponItemUI itemUI))
            {
                return;
            }

            if (itemUI.ItemUIType == m_ItemUIType)
            {
                m_InventoryUI.SwapWeaponItemUIs(itemUI.SlotIndex, SlotIndex);
            }*/

            if (eventData.pointerDrag.TryGetComponent(out SightItemUI sightItemUI))
            {
                // another SightItemUI is being dropped on this SightItemUI
            }
            else if (eventData.pointerDrag.TryGetComponent(out ItemUI itemUI))
            {
                // an ItemUI is being dropped on this SightItemUI
                // Check WeaponInventoryUI for 
                // Check if the ItemUI is of the same type as this SightItemUI
                // if yes,
                // Get GunItemData from the WeaponInventoryUI, and check if this SightItem can be attached to the GunItem
                // if yes,
                // then remove this SightItem from the inventory
                // Then hide the ItemUI and store it in a member variable
                // then set the ItemUI's datas to this SightItemUI and Show it
                // then set this attachment to the GunItem
            }
        }

        public void SetSlotIndex(int index)
        {
            SlotIndex = index;
        }

        public void SetItemDataAndShow(GunItem item)
        {
            // m_SightItem = item;
            ItemData = item.ItemData;
            m_Icon.sprite = ItemData.IconSprite;

            Show();
        }

        void ResetItemDataAndHide()
        {
            m_SightItem = null;
            ItemData = null;
            m_Icon.sprite = null;

            Hide();
        }

        private void Show()
        {
            m_CanvasGroup.alpha = 1;
        }

        private void Hide()
        {
            m_CanvasGroup.alpha = 0;
        }
    }
}
