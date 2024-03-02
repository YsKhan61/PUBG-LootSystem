using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class AttachmentItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        /*[SerializeField]
        ItemUIType m_ItemUIType;
        public ItemUIType ItemUIType => m_ItemUIType;*/

        [SerializeField]
        Image m_Icon;

        [SerializeField]
        AttachmentUIMediator m_Mediator;

        [SerializeField]
        int m_SlotIndex;
        public int SlotIndex => m_SlotIndex;

        private IWeaponAttachment m_StoredItem;
        public IWeaponAttachment StoredItem => m_StoredItem;

        [HideInInspector]
        public bool IsDragSuccess;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;
        private Vector2 m_lastAnchoredPosition;

        /// <summary>
        /// The ItemUI of the SightItem that is being dragged and dropped on this ItemUI
        /// We need to store this, and later we use it to add the SightItem to the inventory
        /// </summary>
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
                if (StoredItem == null)
                    return;

                StoredItem.DetachFromWeapon();
                ShowItemUIAndResetItsPosition();
                ResetDataAndHideAttachmentItemUI();

                m_Mediator.RaiseEventOnInventoryUIItemAdded(m_StoredItem as InventoryItem);
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

            if (eventData.pointerDrag.TryGetComponent(out AttachmentItemUI droppedAttachmentItemUI))
            {
                // another ItemUI is being dropped on this ItemUI

                // return if the dropped ItemUI's ItemUIType is not compatible with this ItemUI's ItemUIType
                if (droppedAttachmentItemUI.m_Mediator.ItemUIType != m_Mediator.ItemUIType)
                {
                    return;
                }

                // if no,
                // Try to get GunItemData from the WeaponInventoryUI
                if (!m_Mediator.WeaponInventoryUI.TryGetWeaopnItemFromWeaponInventoryUI(
                    SlotIndex, out WeaponItem gunInThisSlot))
                {
                    // if GunItem is not found, then return
                    return;
                }

                // Check if the SightItem of dropped ItemUI is same as the SightItem of this ItemUI
                if (StoredItem != null && droppedAttachmentItemUI.StoredItem.ItemData.ItemTag == StoredItem.ItemData.ItemTag)
                {
                    return;
                }

                // if GunItem is found, check if the SightItem of dropped ItemUI can be attached to the GunItem
                if (!droppedAttachmentItemUI.StoredItem.IsWeaponCompatible(gunInThisSlot.WeaponData))
                {
                    return;
                }

                // get the gun item from the WeaponInventoryUI using the SlotIndex of dropped ItemUI
                if (!m_Mediator.WeaponInventoryUI.TryGetWeaopnItemFromWeaponInventoryUI(
                                       droppedAttachmentItemUI.SlotIndex, out WeaponItem gunInDroppedSlot))
                {
                    return;
                }

                // detach the Item of dropped ItemUI from it's GunItem
                droppedAttachmentItemUI.StoredItem.DetachFromWeapon();
                
                //then check if the GunItem of this slot already has a SightItem attached to it (also SightItem can be present in this class)
                if (StoredItem != null)
                {
                    // detach it from GunItem of this slot
                    StoredItem.DetachFromWeapon();
                }

                // add the SightItem of the dropped ItemUI to this gun
                droppedAttachmentItemUI.StoredItem.AttachToWeapon(gunInThisSlot);

                // Get the parent of the dropped ItemUI and the slot index of the dropped ItemUI before dropping it
                Transform parentOfDroppedItemUI = droppedAttachmentItemUI.transform.parent;
                int slotIndexOfDroppedItemUI = droppedAttachmentItemUI.SlotIndex;

                // Add the dropped ItemUI to the SlotUI of this ItemUI through SightInventoryUIMediator
                m_Mediator.DropAttachmentItemUIToSlot(droppedAttachmentItemUI, transform.parent, SlotIndex);

                // if the SightItem of this slot is present
                if (StoredItem != null)
                {
                    // Check if the temp SightItem is compatible with the GunItem of the dropped ItemUI
                    if (droppedAttachmentItemUI.StoredItem.IsWeaponCompatible(gunInDroppedSlot.WeaponData))
                    {
                        // if yes, then add the temp SightItem to the GunItem of the dropped ItemUI
                        StoredItem.AttachToWeapon(gunInDroppedSlot);
                    }
                    else
                    {
                        m_Mediator.RaiseEventOnInventoryUIItemAdded(droppedAttachmentItemUI.StoredItem as InventoryItem);

                        // Make sure to reset the ItemUI's ItemUI's position to the last anchored position
                        ShowItemUIAndResetItsPosition();

                        ResetDataAndHideAttachmentItemUI();
                    }
                }

                // Drop this ItemUI to the dropped ItemUI's Slot
                m_Mediator.DropAttachmentItemUIToSlot(this, parentOfDroppedItemUI, slotIndexOfDroppedItemUI);
            }
            else if (eventData.pointerDrag.TryGetComponent(out ItemUI droppedItemUI))
            {
                // an ItemUI is being dropped on this ItemUI

                // NOTE - We need to do two checks here -
                // 1. Check if the ItemUI is of the same type as this ItemUI -> ItemUI can be dropped on another SightItemUI or SightSlotUI
                // 2. Check if the SightItem can be attached to the GunItem -> 8x scope (SightItem) cannot be attached to a Pistol (GunItem)

                // Check if the ItemUI is of the same type as this ItemUI
                if (droppedItemUI.Item.ItemData.UIType != m_Mediator.ItemUIType)
                {
                    return;
                }

                // Try to get GunItemData from the WeaponInventoryUI
                if (!m_Mediator.WeaponInventoryUI.TryGetWeaopnItemFromWeaponInventoryUI(
                    SlotIndex, out WeaponItem gunInThisSlot))
                {
                    return;
                }

                // if GunItem is found, check if the SightItem of dropped ItemUI can be attached to the GunItem
                if (!(droppedItemUI.Item as IWeaponAttachment).IsWeaponCompatible(gunInThisSlot.WeaponData))
                {
                    return;
                }

                // if yes,
                // then check if the GunItem already has a SightItem attached to it (also SightItem can be present in this class)
                if (StoredItem != null)
                {
                    // detach it from GunItem
                    StoredItem.DetachFromWeapon();

                    // add the SightItem to the inventory
                    m_Mediator.RaiseEventOnInventoryUIItemAdded(m_StoredItem as InventoryItem);
                    
                    if (m_ItemUI == null)
                    {
                        // If the ItemUI is null, then throw an error as every ItemUI should have an ItemUI
                        Debug.LogError("This should not be null");
                    }
                    else
                    {
                        // Show the ItemUI if it is not null, and block raycast, and fallback to last position
                        ShowItemUIAndResetItsPosition();
                    }
                }

                // Then remove the SightItem of dropped ItemUI from the inventory using an event
                m_Mediator.RaiseEventOnInventoryUIItemRemoved(droppedItemUI.Item);

                // Then hide the ItemUI and store it in a member variable
                HideItemUI(droppedItemUI);

                // then set the ItemUI's datas to this ItemUI and Show it
                SetDataAndShowAttachmentItemUI(droppedItemUI.Item as IWeaponAttachment);

                // then set this attachment to the GunItem
                (droppedItemUI.Item as IWeaponAttachment).AttachToWeapon(gunInThisSlot);
            }
        }

        public void SetSlotIndex(int index)
        {
            m_SlotIndex = index;
        }

        internal void SetDataAndShowAttachmentItemUI(IWeaponAttachment item)
        {
            m_StoredItem = item;
            m_Icon.sprite = item.ItemData.IconSprite;

            Show();
        }

        internal void ResetDataAndHideAttachmentItemUI()
        {
            m_StoredItem = null;
            m_Icon.sprite = null;

            Hide();
        }

        internal void ShowItemUIAndResetItsPosition()
        {
            if (m_ItemUI == null)
            {
                // If the ItemUI is null, then throw an error as every ItemUI should have an ItemUI
                Debug.LogError("This should not be null");
                return;
            }
            else
            {
                // Show the ItemUI if it is not null, and block raycast, and fallback to last position
                m_ItemUI.Show();
                m_ItemUI.BlockRaycast();
                m_ItemUI.FallbackToLastPosition();
            }
        }

        /// <summary>
        /// Hide and reset the ItemUI of the SightItem 
        /// that is being dragged and dropped on this ItemUI
        /// </summary>
        /// <param name="itemUI"></param>
        void HideItemUI(ItemUI itemUI)
        {
            // itemUI.IsDragSuccess = true;            // Set the drag success to true, so that the OnEndDrag of ItemUI doesn't make it visible again or fallback to last position
            itemUI.Hide();
            itemUI.UnblockRaycast();
            m_ItemUI = itemUI;
        }

        internal void Show()
        {
            m_CanvasGroup.alpha = 1;
        }

        internal void ShowSlot()
        {
            if (transform.parent.TryGetComponent(out CanvasGroup cg))
            {
                cg.alpha = 1;
                cg.blocksRaycasts = true;
            }
        }

        internal void Hide()
        {
            m_CanvasGroup.alpha = 0;
        }

        internal void HideSlot()
        {
            if (transform.parent.TryGetComponent(out CanvasGroup cg))
            {
                cg.alpha = 0;
                cg.blocksRaycasts = false;
            }
        }
    }
}
