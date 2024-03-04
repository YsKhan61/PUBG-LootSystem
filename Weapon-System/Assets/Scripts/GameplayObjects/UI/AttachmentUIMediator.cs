
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
    public class AttachmentUIMediator : MonoBehaviour
    {
        /// <remark>
        /// Keep this field on top, so that it's recognizable in the inspector
        /// </remark>
        [SerializeField, Tooltip("The type of this instance of the class!")]
        ItemUIType m_ItemUIType;
        public ItemUIType ItemUIType => m_ItemUIType;


        [Space(10)]

        [Header("Broadcast to")]


        [SerializeField, Tooltip("To add an inventory item in the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnAddInventoryItemToInventoryEvent;

        [SerializeField, Tooltip("To remove an inventory item from inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnRemoveInventoryItemFromInventoryEvent;


        [Space(10)]

        [Header("Listens to")]

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked, Listen to this event to add a WeaponItemUI in respective WeaponSlotUI")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToInventoryEvent;

        [SerializeField, Tooltip("Before an Weaopn item is removed from the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other in the inventory, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemSwappedInInventoryEvent;


        [Space(10)]

        [SerializeField, FormerlySerializedAs("m_WeaponInventoryUI")]
        WeaponUIMediator m_WeaponUIMediator;
        public WeaponUIMediator WeaponInventoryUI => m_WeaponUIMediator;

        [SerializeField]
        AttachmentItemUI[] m_AttachmentItemUIs;



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

        internal void AddAttachmentItemToInventory(InventoryItem item)
        {
            m_OnAddInventoryItemToInventoryEvent.RaiseEvent(item);
        }

        private void RemoveAttachmentItemFromInventory(InventoryItem item)
        {
            m_OnRemoveInventoryItemFromInventoryEvent.RaiseEvent(item);
        }

        internal void OnRightClickInputOnAttachmentItemUI(AttachmentItemUI attachmentItemUI)
        {
            if (attachmentItemUI.StoredItem == null)
                return;

            attachmentItemUI.StoredItem.DetachFromWeapon();
            AddAttachmentItemToInventory(attachmentItemUI.StoredItem as InventoryItem);
            // ShowItemUIAndResetItsPosition();
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
            if (droppedAttachmentItemUI.Mediator.ItemUIType != ItemUIType)
            {
                return;
            }

            // if no,
            // Try to get GunItemData from the WeaponInventoryUI
            if (!WeaponInventoryUI.TryGetWeaopnItemFromWeaponInventoryUI(
                attachmentItemUIOfDropArea.SlotIndex, out WeaponItem weaponItemInThisDropArea))
            {
                // if GunItem is not found, then return
                return;
            }

            // Check if the SightItem of dropped ItemUI is same as the SightItem of this ItemUI
            if (attachmentItemUIOfDropArea.StoredItem != null && 
                droppedAttachmentItemUI.StoredItem.ItemData.ItemTag == attachmentItemUIOfDropArea.StoredItem.ItemData.ItemTag)
            {
                return;
            }

            // if GunItem is found, check if the SightItem of dropped ItemUI can be attached to the GunItem
            if (!droppedAttachmentItemUI.StoredItem.IsWeaponCompatible(weaponItemInThisDropArea.WeaponData))
            {
                return;
            }

            // get the gun item from the WeaponInventoryUI using the SlotIndex of dropped ItemUI
            if (WeaponInventoryUI.TryGetWeaopnItemFromWeaponInventoryUI(
                                   droppedAttachmentItemUI.SlotIndex, out WeaponItem weaponItemOfDroppedAttachmentItemUI))
            {
                return;
            }

            // detach the Item of dropped ItemUI from it's GunItem
            droppedAttachmentItemUI.StoredItem.DetachFromWeapon();

            //then check if the GunItem of this slot already has a SightItem attached to it (also SightItem can be present in this class)
            if (attachmentItemUIOfDropArea.StoredItem != null)
            {
                // detach it from GunItem of this slot
                attachmentItemUIOfDropArea.StoredItem.DetachFromWeapon();
            }

            // add the SightItem of the dropped ItemUI to this gun
            droppedAttachmentItemUI.StoredItem.AttachToWeapon(weaponItemInThisDropArea);

            // Get the parent of the dropped ItemUI and the slot index of the dropped ItemUI before dropping it
            Transform parentOfDroppedItemUI = droppedAttachmentItemUI.transform.parent;
            int slotIndexOfDroppedItemUI = droppedAttachmentItemUI.SlotIndex;

            // Add the dropped ItemUI to the SlotUI of this ItemUI through SightInventoryUIMediator
            DropAttachmentItemUIToSlot(droppedAttachmentItemUI, transform.parent, attachmentItemUIOfDropArea.SlotIndex);

            // if the SightItem of this slot is present
            if (attachmentItemUIOfDropArea.StoredItem != null)
            {
                // Check if the temp SightItem is compatible with the GunItem of the dropped ItemUI
                if (droppedAttachmentItemUI.StoredItem.IsWeaponCompatible(weaponItemOfDroppedAttachmentItemUI.WeaponData))
                {
                    // if yes, then add the temp SightItem to the GunItem of the dropped ItemUI
                    attachmentItemUIOfDropArea.StoredItem.AttachToWeapon(weaponItemOfDroppedAttachmentItemUI);
                }
                else
                {
                    AddAttachmentItemToInventory(droppedAttachmentItemUI.StoredItem as InventoryItem);

                    // Make sure to reset the ItemUI's ItemUI's position to the last anchored position
                    /*ShowItemUIAndResetItsPosition();*/

                    droppedAttachmentItemUI.ResetDataAndHideAttachmentItemUI();
                }
            }

            // Drop this ItemUI to the dropped ItemUI's Slot
            DropAttachmentItemUIToSlot(attachmentItemUIOfDropArea, parentOfDroppedItemUI, slotIndexOfDroppedItemUI);
        }

        internal void OnItemUIDroppedInAttachmentItemUI(ItemUI droppedItemUI, AttachmentItemUI attachmentItemUI)
        {
            // an ItemUI is being dropped on this ItemUI

            // NOTE - We need to do two checks here -
            // 1. Check if the ItemUI is of the same type as this ItemUI -> ItemUI can be dropped on another SightItemUI or SightSlotUI
            // 2. Check if the SightItem can be attached to the GunItem -> 8x scope (SightItem) cannot be attached to a Pistol (GunItem)

            // Check if the ItemUI is of the same type as this ItemUI
            if (droppedItemUI.Item.ItemData.UIType != ItemUIType)
            {
                return;
            }

            // Try to get WeaponItemData from the WeaponInventoryUI
            if (!WeaponInventoryUI.TryGetWeaopnItemFromWeaponInventoryUI(
                attachmentItemUI.SlotIndex, out WeaponItem weaponItemInTheSlotOfDropArea))
            {
                return;
            }

            // if GunItem is found, check if the SightItem of dropped ItemUI can be attached to the GunItem
            if (!(droppedItemUI.Item as IWeaponAttachment).IsWeaponCompatible(weaponItemInTheSlotOfDropArea.WeaponData))
            {
                return;
            }

            // if yes,
            // then check if the WeaponItem already has a SightItem attached to it (also SightItem can be present in this class)
            if (attachmentItemUI.StoredItem != null)
            {
                // detach it from GunItem
                attachmentItemUI.StoredItem.DetachFromWeapon();

                // add the SightItem to the inventory
                AddAttachmentItemToInventory(attachmentItemUI.StoredItem as InventoryItem);

                /*if (attachmentItemUI.TempItemUI == null)
                {
                    // If the ItemUI is null, then throw an error as every ItemUI should have an ItemUI
                    Debug.LogError("This should not be null");
                }
                else
                {
                    // Show the ItemUI if it is not null, and block raycast, and fallback to last position
                    attachmentItemUI.ShowItemUIAndResetItsPosition();
                }*/
            }

            // Then remove the SightItem of dropped ItemUI from the inventory using an event
            RemoveAttachmentItemFromInventory(droppedItemUI.Item);

            // Then hide the ItemUI and store it in a member variable
            // HideItemUI(droppedItemUI);

            // then set the ItemUI's datas to this ItemUI and Show it
            attachmentItemUI.SetDataAndShowAttachmentItemUI(droppedItemUI.Item as IWeaponAttachment);

            // then set this attachment to the GunItem
            (droppedItemUI.Item as IWeaponAttachment).AttachToWeapon(weaponItemInTheSlotOfDropArea);
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
            if (!TryGetAttachmentItemUIFromSlotIndex(slotIndex, out AttachmentItemUI itemUI))
            {
                return;
            }

            // If no attachment is attached to the weapon
            if (itemUI.StoredItem == null)
                return;

            itemUI.StoredItem.DetachFromWeapon();
            itemUI.ShowItemUIAndResetItsPosition();
            itemUI.ResetDataAndHideAttachmentItemUI();
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
        private bool IsWeaponCompatible(in WeaponDataSO weaponData)
        {
            switch (m_ItemUIType)
            {
                case ItemUIType.SightAttachment:
                    return weaponData.AllowedSightAttachments.Length > 0;
                case ItemUIType.MuzzleAttachment:
                    return weaponData.AllowedMuzzleAttachments.Length > 0;
                case ItemUIType.GripAttachment:
                    return weaponData.AllowedGripAttachments.Length > 0;
                case ItemUIType.MagazineAttachment:
                    return weaponData.AllowedMagazineAttachments.Length > 0;
                case ItemUIType.StockAttachment:
                    return weaponData.AllowedStockAttachments.Length > 0;
            }

            return false;
        }
    }
}
