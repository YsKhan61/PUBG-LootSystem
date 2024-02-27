using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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

        [SerializeField]
        SightInventoryUIMediator m_SightInventoryUIMediator;

        [SerializeField]
        int m_SlotIndex;
        public int SlotIndex => m_SlotIndex;

        private SightAttachmentItem m_StoredSightItem;
        public SightAttachmentItem StoredSightItem => m_StoredSightItem;

        [HideInInspector]
        public bool IsDragSuccess;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;
        private Vector2 m_lastAnchoredPosition;

        /// <summary>
        /// The ItemUI of the SightItem that is being dragged and dropped on this SightItemUI
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
                // Remove the SightItem from the gun
                if (!m_SightInventoryUIMediator.WeaponInventoryUI.TryGetGunItemFromWeaponInventoryUI(
                    SlotIndex, out GunItem gun))
                {
                    // right clicked on an empty SightItemUI
                    return;
                }

                gun.DetachSight();

                // Store back the SightItem in the inventory
                m_SightInventoryUIMediator.AddItemToInventory(m_StoredSightItem);

                // Make sure to reset the ItemUI's position to the last anchored position
                ShowItemUIAndResetItsPosition();

                ResetDataAndHideSightItemUI();
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

            if (eventData.pointerDrag.TryGetComponent(out SightItemUI droppedSightItemUI))
            {
                // another SightItemUI is being dropped on this SightItemUI

                // return if the dropped SightItemUI's ItemUIType is not compatible with this SightItemUI's ItemUIType
                if (droppedSightItemUI.ItemUIType != ItemUIType)
                {
                    return;
                }

                // if no,
                // Try to get GunItemData from the WeaponInventoryUI
                if (!m_SightInventoryUIMediator.WeaponInventoryUI.TryGetGunItemFromWeaponInventoryUI(
                    SlotIndex, out GunItem gunInThisSlot))
                {
                    // if GunItem is not found, then return
                    return;
                }

                bool isSightPresentInThisSlot =
                    StoredSightItem != null &&
                    gunInThisSlot.SightAttachment != null &&
                    gunInThisSlot.SightAttachment as SightAttachmentItem == StoredSightItem;


                // Check if the SightItem of dropped SightItemUI is same as the SightItem of this SightItemUI
                if (isSightPresentInThisSlot && droppedSightItemUI.StoredSightItem.ItemData.ItemTag == StoredSightItem.ItemData.ItemTag)
                {
                    return;
                }

                // if GunItem is found, check if the SightItem of dropped SightItemUI can be attached to the GunItem
                if (!gunInThisSlot.IsSightTypeCompatible(droppedSightItemUI.StoredSightItem.ItemData.ItemTag))
                {
                    return;
                }

                // get the gun item from the WeaponInventoryUI using the SlotIndex of dropped SightItemUI
                if (!m_SightInventoryUIMediator.WeaponInventoryUI.TryGetGunItemFromWeaponInventoryUI(
                                       droppedSightItemUI.SlotIndex, out GunItem gunInDroppedSlot))
                {
                    return;
                }

                // detach the SightItem of dropped SightItemUI from it's GunItem
                gunInDroppedSlot.DetachSight();
                
                //then check if the GunItem of this slot already has a SightItem attached to it (also SightItem can be present in this class)
                if (isSightPresentInThisSlot)
                {
                    if (gunInThisSlot.SightAttachment != m_StoredSightItem)
                    {
                        Debug.LogError("This should not happen as the Gun's sight attachment must match with the stored Sight Attachment Item");
                        return;
                    }
                    // detach it from GunItem of this slot
                    gunInThisSlot.DetachSight();
                }

                // add the SightItem of the dropped SightItemUI to this gun
                gunInThisSlot.AttachSight(droppedSightItemUI.StoredSightItem);

                // Get the parent of the dropped SightItemUI and the slot index of the dropped SightItemUI before dropping it
                Transform parentOfDroppedItemUI = droppedSightItemUI.transform.parent;
                int slotIndexOfDroppedItemUI = droppedSightItemUI.SlotIndex;

                // Add the dropped SightItemUI to the SlotUI of this SightItemUI through SightInventoryUIMediator
                // m_SightInventoryUI.AddSightItemUIToSightSlotUI(droppedSightItemUI, SlotIndex);
                m_SightInventoryUIMediator.DropSightItemUIToSlot(droppedSightItemUI, transform.parent, SlotIndex);

                // if the SightItem of this slot is present
                if (isSightPresentInThisSlot)
                {
                    // Check if the temp SightItem is compatible with the GunItem of the dropped SightItemUI
                    if (gunInDroppedSlot.IsSightTypeCompatible(StoredSightItem.ItemData.ItemTag))
                    {
                        // if yes, then add the temp SightItem to the GunItem of the dropped SightItemUI
                        gunInDroppedSlot.AttachSight(StoredSightItem);
                    }
                    else
                    {
                        m_SightInventoryUIMediator.AddItemToInventory(droppedSightItemUI.StoredSightItem);

                        // Make sure to reset the droppedSightItemUI's ItemUI's position to the last anchored position
                        ShowItemUIAndResetItsPosition();

                        ResetDataAndHideSightItemUI();
                    }
                }

                // Drop this SightItemUI to the dropped SightItemUI's Slot
                m_SightInventoryUIMediator.DropSightItemUIToSlot(this, parentOfDroppedItemUI, slotIndexOfDroppedItemUI);
            }
            else if (eventData.pointerDrag.TryGetComponent(out ItemUI droppedItemUI))
            {
                // an ItemUI is being dropped on this SightItemUI

                // NOTE - We need to do two checks here -
                // 1. Check if the ItemUI is of the same type as this SightItemUI -> SightItemUI can be dropped on another SightItemUI or SightSlotUI
                // 2. Check if the SightItem can be attached to the GunItem -> 8x scope (SightItem) cannot be attached to a Pistol (GunItem)

                // Check if the ItemUI is of the same type as this SightItemUI
                if (droppedItemUI.ItemData.UIType != ItemUIType)
                {
                    return;
                }

                // Try to get GunItemData from the WeaponInventoryUI
                if (!m_SightInventoryUIMediator.WeaponInventoryUI.TryGetGunItemFromWeaponInventoryUI(
                    SlotIndex, out GunItem gunInThisSlot))
                {
                    return;
                }

                // if GunItem is found, check if the SightItem of dropped SightItemUI can be attached to the GunItem
                if (!gunInThisSlot.IsSightTypeCompatible(droppedItemUI.ItemData.ItemTag))
                {
                    return;
                }

                // if yes,
                // then check if the GunItem already has a SightItem attached to it (also SightItem can be present in this class)
                if (gunInThisSlot.SightAttachment != null || StoredSightItem != null)
                {
                    if (gunInThisSlot.SightAttachment != StoredSightItem)
                    {
                        Debug.LogError("This should not happen as the Gun's sight attachment must match with the stored Sight Attachment Item");
                        return;
                    }

                    // detach it from GunItem
                    gunInThisSlot.DetachSight();

                    // remove the SightItemUI from SightSlotUI through SightInventoryUI
                    // m_SightInventoryUI.RemoveSightItemUIFromSightSlotUI(m_StoredSightItem);

                    // add the SightItem to the inventory
                    m_SightInventoryUIMediator.AddItemToInventory(m_StoredSightItem);
                    
                    if (m_ItemUI == null)
                    {
                        // If the ItemUI is null, then throw an error as every SightItemUI should have an ItemUI
                        Debug.LogError("This should not be null");
                    }
                    else
                    {
                        // Show the ItemUI if it is not null, and block raycast, and fallback to last position
                        ShowItemUIAndResetItsPosition();
                    }
                }

                // Then remove the SightItem of dropped ItemUI from the inventory using an event
                m_SightInventoryUIMediator.RemoveItemFromInventory(droppedItemUI.Item);

                // Then hide the ItemUI and store it in a member variable
                HideItemUI(droppedItemUI);
                
                // then set the ItemUI's datas to this SightItemUI and Show it
                SetDataAndShowSightItemUI(droppedItemUI.Item as SightAttachmentItem);

                // then set this attachment to the GunItem
                gunInThisSlot.AttachSight(droppedItemUI.Item as SightAttachmentItem);
            }
        }

        public void SetSlotIndex(int index)
        {
            m_SlotIndex = index;
        }

        public void SetDataAndShowSightItemUI(SightAttachmentItem item)
        {
            m_StoredSightItem = item;
            m_Icon.sprite = item.ItemData.IconSprite;

            Show();
        }


        

        void ResetDataAndHideSightItemUI()
        {
            m_StoredSightItem = null;
            m_Icon.sprite = null;

            Hide();
        }

        void HideItemUI(ItemUI itemUI)
        {
            itemUI.IsDragSuccess = true;            // Set the drag success to true, so that the OnEndDrag of ItemUI doesn't make it visible again or fallback to last position
            itemUI.Hide();
            itemUI.UnblockRaycast();
            m_ItemUI = itemUI;
        }

        void ShowItemUIAndResetItsPosition()
        {
            if (m_ItemUI == null)
            {
                // If the ItemUI is null, then throw an error as every SightItemUI should have an ItemUI
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
