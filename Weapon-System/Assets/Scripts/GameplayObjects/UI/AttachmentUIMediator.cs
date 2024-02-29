
using UnityEngine;
using UnityEngine.Serialization;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the sight attachment UIs in the WeaponSlotUIs. [Drag drop, swap, remove, add etc]
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

        [SerializeField, Tooltip("When an Inventory Item UI is added to the InventoryUI, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemUIAddedEvent;

        [SerializeField, Tooltip("When an Inventory item UI is removed from the inventory UI, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemUIRemovedEvent;


        [Space(10)]

        [Header("Listens to")]

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked, Listen to this event to add a WeaponItemUI in respective WeaponSlotUI")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToInventoryEvent;

        [SerializeField, Tooltip("When an WeaponItemUI is going to be removed from WeaponInventoryUI, this event is invoked before that happens")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemUIRemovedEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemUISwappedEvent;


        [Space(10)]

        [SerializeField, FormerlySerializedAs("m_WeaponInventoryUI")]
        WeaponUIMediator m_WeaponUIMediator;
        public WeaponUIMediator WeaponInventoryUI => m_WeaponUIMediator;

        [SerializeField]
        AttachmentItemUI[] m_AttachmentItemUIs;



        private void Start()
        {
            m_OnWeaponItemAddedToInventoryEvent.OnEventRaised += ConfigureAttachmentItemUI;
            m_OnBeforeWeaponItemUIRemovedEvent.OnEventRaised += DetachAttachmentFromWeaponAndResetUI;
            m_OnWeaponItemUISwappedEvent.OnEventRaised += SwapSlotIndices;
        }

        private void OnDestroy()
        {
            m_OnWeaponItemAddedToInventoryEvent.OnEventRaised += ConfigureAttachmentItemUI;
            m_OnBeforeWeaponItemUIRemovedEvent.OnEventRaised += DetachAttachmentFromWeaponAndResetUI;
            m_OnWeaponItemUISwappedEvent.OnEventRaised -= SwapSlotIndices;
        }

        public void RaiseEventOnInventoryUIItemAdded(InventoryItem item)
        {
            m_OnInventoryItemUIAddedEvent.RaiseEvent(item);
        }

        public void RaiseEventOnInventoryUIItemRemoved(InventoryItem item)
        {
            m_OnInventoryItemUIRemovedEvent.RaiseEvent(item);
        }

        /// <summary>
        /// Drop the other SightItemUI to this SightItemUI
        /// </summary>
        /// <param name="itemUI">the other SightItemUI</param>
        public void DropAttachmentItemUIToSlot(AttachmentItemUI itemUI, Transform slotTransform, int slotIndex)
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

            m_OnInventoryItemUIAddedEvent.RaiseEvent(itemUI.StoredItem as InventoryItem);
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

        bool TryGetAttachmentItemUIFromSlotIndex(int slotIndex, out AttachmentItemUI itemUI)
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
