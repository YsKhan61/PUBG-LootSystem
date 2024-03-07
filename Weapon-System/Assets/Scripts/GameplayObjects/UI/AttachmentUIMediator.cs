
using UnityEngine;
using UnityEngine.Serialization;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the AttachmentItemUIs of different types with their respective AttachmentUIMediator
    /// eg: SightAttachment, MuzzleAttachment, GripAttachment, MagazineAttachment, StockAttachment
    /// NOTE - Every type of AttachmentItemUI should have a separate AttachmentUIMediator
    /// </summary>
    public abstract class AttachmentUIMediator : MonoBehaviour
    {
        /// <remark>
        /// Keep this field on top, so that it's recognizable in the inspector
        /// </remark>
        [SerializeField, Tooltip("The type of this instance of the class!")]
        ItemUITagSO m_ItemUITag;
        public ItemUITagSO ItemUITag => m_ItemUITag;


        [Space(10)]

        [Header("Listens to")]

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked, Listen to this event to add a WeaponItemUI in respective WeaponSlotUI")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToInventoryEvent;

        [SerializeField, Tooltip("Before an Weaopn item is removed from the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other in the inventory, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemSwappedInInventoryEvent;


        [Space(10)]

        [SerializeField]
        ItemUserHand m_ItemUserHand;

        [SerializeField]
        AttachmentItemUI[] m_AttachmentItemUIs;

        [SerializeField]
        Transform m_CanvasTransform;
        public Transform CanvasTransform => m_CanvasTransform;



        private void Start()
        {
            m_OnWeaponItemAddedToInventoryEvent.OnEventRaised += ConfigureAttachmentItemUI;
            m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent.OnEventRaised += DetachAttachmentFromWeaponAndResetUI;
            m_OnWeaponItemSwappedInInventoryEvent.OnEventRaised += SwapSlotIndices;
        }

        private void OnDestroy()
        {
            m_OnWeaponItemAddedToInventoryEvent.OnEventRaised += ConfigureAttachmentItemUI;
            m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent.OnEventRaised += DetachAttachmentFromWeaponAndResetUI;
            m_OnWeaponItemSwappedInInventoryEvent.OnEventRaised -= SwapSlotIndices;
        }

        internal void OnRightClickInputOnAttachmentItemUI(AttachmentItemUI attachmentItemUI)
        {
            if (attachmentItemUI.StoredItem == null)
                return;

            attachmentItemUI.StoredItem.DetachFromWeapon();
            m_ItemUserHand.TryStoreInventoryItemAndRaiseEvent(attachmentItemUI.StoredItem as InventoryItem);
            attachmentItemUI.ResetDataAndHideAttachmentItemUI();
        }

        internal void OnAttachmentItemUIDroppedInAttachmentItemUI(AttachmentItemUI droppedAttachmentItemUI, AttachmentItemUI attachmentItemUIOfDropArea)
        {
            // If an empty AttachmentItemUI is dropped on this AttachmentItemUI, return
            if (droppedAttachmentItemUI.StoredItem == null)
            {
                return;
            }

            // return if the dropped ItemUI's ItemUIType is not compatible with this ItemUI's ItemUIType
            if (droppedAttachmentItemUI.Mediator.ItemUITag != ItemUITag)
            {
                return;
            }

            // Check if the AttachmentItem of dropped AttachmentItemUI is same as the AttachmentItem of this drop area
            if (attachmentItemUIOfDropArea.StoredItem != null &&
                droppedAttachmentItemUI.StoredItem.ItemData.ItemTag == attachmentItemUIOfDropArea.StoredItem.ItemData.ItemTag)
            {
                return;
            }

            // Try to get WeaponItem using the SlotIndex of this attachmentItemUIOfDropArea
            if (!m_ItemUserHand.TryGetWeaponItemFromWeaponInventory(
                attachmentItemUIOfDropArea.SlotIndex, out WeaponItem weaponItemInThisDropArea))
            {
                return;
            }

            // if WeaponItem is found, check if the AttachmentItem of droppedAttachmentItemUI can be attached to the WeaponItem
            if (!droppedAttachmentItemUI.StoredItem.IsWeaponCompatible(weaponItemInThisDropArea.WeaponData))
            {
                return;
            }

            // Try to get WeaponItem using the SlotIndex of the droppedAttachmentItemUI
            if (!m_ItemUserHand.TryGetWeaponItemFromWeaponInventory(
                droppedAttachmentItemUI.SlotIndex, out WeaponItem weaponItemOfDroppedAttachmentItemUI))
            {
                return;
            }

            // detach the Item of dropped ItemUI from it's GunItem
            droppedAttachmentItemUI.StoredItem.DetachFromWeapon();

            //then check if the GunItem of this slot already has an attachment item attached to it
            if (attachmentItemUIOfDropArea.StoredItem != null)
            {
                // detach it from GunItem of this slot
                attachmentItemUIOfDropArea.StoredItem.DetachFromWeapon();
            }

            // add the SightItem of the dropped ItemUI to this gun
            droppedAttachmentItemUI.StoredItem.AttachToWeapon(weaponItemInThisDropArea);

            // Get the parent of the dropped ItemUI and the slot index of the dropped ItemUI before dropping it
            Transform parentOfDroppedItemUI = droppedAttachmentItemUI.LastParent;
            int slotIndexOfDroppedItemUI = droppedAttachmentItemUI.SlotIndex;

            DropAttachmentItemUIToSlot(droppedAttachmentItemUI, attachmentItemUIOfDropArea.transform.parent, attachmentItemUIOfDropArea.SlotIndex);

            // if the AttachmentItem of this slot is present
            if (attachmentItemUIOfDropArea.StoredItem != null)
            {
                // Check if the temp SightItem is compatible with the GunItem of the dropped ItemUI
                if (attachmentItemUIOfDropArea.StoredItem.IsWeaponCompatible(weaponItemOfDroppedAttachmentItemUI.WeaponData))
                {
                    attachmentItemUIOfDropArea.StoredItem.AttachToWeapon(weaponItemOfDroppedAttachmentItemUI);
                }
                else
                {
                    m_ItemUserHand.TryStoreInventoryItemAndRaiseEvent(droppedAttachmentItemUI.StoredItem as InventoryItem);
                    droppedAttachmentItemUI.ResetDataAndHideAttachmentItemUI();
                }
            }

            // Drop this ItemUI to the dropped ItemUI's Slot
            DropAttachmentItemUIToSlot(attachmentItemUIOfDropArea, parentOfDroppedItemUI, slotIndexOfDroppedItemUI);
        }

        internal void OnItemUIDroppedInAttachmentItemUI(ItemUI droppedItemUI, AttachmentItemUI attachmentItemUIOfDropArea)
        {
            // an ItemUI is being dropped on this AttachmentItemUI

            if (droppedItemUI.StoredItem == null)
            {
                return;
            }

            // Check if the ItemUI is of the same type as this ItemUI
            if (droppedItemUI.StoredItem.ItemData.UITag != ItemUITag)
            {
                return;
            }

            // Check if the Item of dropped ItemUI is same as the AttachmentItem of this drop area
            if (attachmentItemUIOfDropArea.StoredItem != null &&
                droppedItemUI.StoredItem.ItemData.ItemTag == attachmentItemUIOfDropArea.StoredItem.ItemData.ItemTag)
            {
                return;
            }

            // Try to get WeaponItemData from the WeaponInventoryUI
            if (!m_ItemUserHand.TryGetWeaponItemFromWeaponInventory(
                attachmentItemUIOfDropArea.SlotIndex, out WeaponItem weaponItemInTheSlotOfDropArea))
            {
                return;
            }

            // if GunItem is found, check if the SightItem of dropped ItemUI can be attached to the GunItem
            if (!(droppedItemUI.StoredItem as IWeaponAttachment).IsWeaponCompatible(weaponItemInTheSlotOfDropArea.WeaponData))
            {
                return;
            }

            // if yes,
            // then check if the WeaponItem already has a SightItem attached to it (also SightItem can be present in this class)
            if (attachmentItemUIOfDropArea.StoredItem != null)
            {
                attachmentItemUIOfDropArea.StoredItem.DetachFromWeapon();

                // We only store the attachment item, as it was collected already when it was picked up.
                m_ItemUserHand.TryStoreInventoryItemAndRaiseEvent(attachmentItemUIOfDropArea.StoredItem as InventoryItem);
            }

            switch (droppedItemUI.StoredSlotType)
            {
                case SlotType.Vicinity:
                    if (!m_ItemUserHand.TryCollectItem(droppedItemUI.StoredItem))
                    {
                        Debug.LogError("This should not happen!");
                        return;
                    }
                    break;

                case SlotType.Inventory:
                    // We only remove it from teh inventory, no need to drop it as it will be attached to weapon
                    if (!m_ItemUserHand.TryRemoveInventoryItem(droppedItemUI.StoredItem))
                    {
                        Debug.LogError("This should not happen!");
                        return;
                    }
                    break;
            }

            (droppedItemUI.StoredItem as IWeaponAttachment).AttachToWeapon(weaponItemInTheSlotOfDropArea);

            attachmentItemUIOfDropArea.SetDataAndShowAttachmentItemUI(droppedItemUI.StoredItem as IWeaponAttachment);

            droppedItemUI.ReleaseSelfToPool();
        }

        /// <summary>
        /// Drop the other AttachmentItemUI to this AttachmentItemUI (dont change datas)
        /// </summary>
        /// <param name="itemUI">the other SightItemUI</param>
        private void DropAttachmentItemUIToSlot(AttachmentItemUI itemUI, Transform slotTransform, int slotIndex)
        {
            itemUI.transform.SetParent(slotTransform);
            itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemUI.IsDragSuccess = true;

            itemUI.SetSlotIndex(slotIndex);
        }

        private void ConfigureAttachmentItemUI(WeaponItem item, int index)
        {
            if (!TryGetAttachmentItemUIFromSlotIndex(index, out AttachmentItemUI attachmentItemUI))
            {
                Debug.LogError("This should not happen!");
                return;
            }

            if (IsWeaponCompatible(item.WeaponData))
            {
                attachmentItemUI.ShowSlot();
            }
            else
            {
                attachmentItemUI.HideSlot();
            }
        }

        private void DetachAttachmentFromWeaponAndResetUI(WeaponItem _, int slotIndex)
        {
            if (!TryGetAttachmentItemUIFromSlotIndex(slotIndex, out AttachmentItemUI attachmentItemUI))
            {
                return;
            }

            // If no attachment is attached to the weapon
            if (attachmentItemUI.StoredItem == null)
                return;

            attachmentItemUI.StoredItem.DetachFromWeapon();
            m_ItemUserHand.TryStoreInventoryItemAndRaiseEvent(attachmentItemUI.StoredItem as InventoryItem);
            attachmentItemUI.ResetDataAndHideAttachmentItemUI();
        }

        private void SwapSlotIndices(int leftIndex, int rightIndex)
        {
            if (!TryGetAttachmentItemUIFromSlotIndex(leftIndex, out AttachmentItemUI leftUI))
            {
                Debug.LogError("This should not happen!");
                return;
            }

            if (!TryGetAttachmentItemUIFromSlotIndex(rightIndex, out AttachmentItemUI rightUI))
            {
                Debug.LogError("This should not happen!");
                return;
            }

            leftUI.SetSlotIndex(rightIndex);
            rightUI.SetSlotIndex(leftIndex);
        }

        private bool TryGetAttachmentItemUIFromSlotIndex(int slotIndex, out AttachmentItemUI itemUI)
        {
            itemUI = null;
            foreach (AttachmentItemUI ui in m_AttachmentItemUIs)
            {
                if (ui.SlotIndex == slotIndex)
                {
                    itemUI = ui;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks for weapon compatibility with attachment UI type
        /// </summary>
        /// <param name="weaponData"></param>
        /// <returns></returns>
        protected abstract bool IsWeaponCompatible(in WeaponDataSO weaponData);
    }
}
