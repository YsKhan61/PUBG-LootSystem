using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class WeaponItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField]
        ItemUIType m_ItemUIType;
        public ItemUIType ItemUIType => m_ItemUIType;

        [SerializeField]
        Image m_Icon;

        [SerializeField]
        TextMeshProUGUI m_NameText;

        public ItemDataSO ItemData { get; private set; }            // not needed, as we are storing the reference to the GunItem in m_GunItem
        public int SlotIndex { get; private set; }

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Vector2 m_lastAnchoredPosition;

        [SerializeField, FormerlySerializedAs("m_InventoryUI")]
        private WeaponInventoryUIMediator m_WeaponInventoryUI;

        
        private GunItem m_StoredGunItem;
        /// <summary>
        /// This stores the GunItem data of this ItemUI, from the Inventory
        /// </summary>
        public GunItem GunItem => m_StoredGunItem;

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
                m_WeaponInventoryUI?.RemoveGunItemUIFromGunSlotUI(m_StoredGunItem);
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

            if (eventData.pointerDrag.TryGetComponent(out WeaponItemUI weaponItemUI))
            {
                if (weaponItemUI.ItemUIType == m_ItemUIType)
                {
                    m_WeaponInventoryUI.SwapWeaponItemUIs(weaponItemUI.SlotIndex, SlotIndex);
                }
            }
            else if (eventData.pointerDrag.TryGetComponent(out ItemUI itemUI))
            {
                if (itemUI.ItemData.UIType == m_ItemUIType)
                {
                    // This is an ItemUI of a Gun Item,
                    // If the index of this ItemUI matches with the index of ItemUI of the currently held Gun Item, then put away the gun from hand.
                    // Drop the Gun Item from the Inventory matchin the index of this ItemUI
                    // Replace the current Gun Item UI datas with the new Gun Item UI datas
                    // Add the new Gun Item to the inventory
                    // If the previous gun was in hand, then add the new gun to the hand
                }
            }
            
        }

        public void SetSlotIndex(int index)
        {
            SlotIndex = index;
        }

        public void SetItemDataAndShow(GunItem item)
        {
            m_StoredGunItem = item;
            ItemData = item.ItemData;
            m_Icon.sprite = ItemData.IconSprite;
            m_NameText.text = ItemData.name;

            Show();
        }

        void ResetItemDataAndHide()
        {
            m_StoredGunItem = null;
            ItemData = null;
            m_Icon.sprite = null;
            m_NameText.text = "";

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
