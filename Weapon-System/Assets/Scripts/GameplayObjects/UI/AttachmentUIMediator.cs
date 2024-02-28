
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the sight attachment UIs in the WeaponSlotUIs. [Drag drop, swap, remove, add etc]
    /// </summary>
    public class AttachmentUIMediator : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("When an WeaponItemUI is going to be removed from WeaponInventoryUI, this event is invoked before that happens")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemUIRemovedEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemUISwappedEvent;

        [Space(10)]

        [SerializeField]
        Inventory m_Inventory;

        [SerializeField]
        WeaponInventoryUIMediator m_WeaponInventoryUI;
        public WeaponInventoryUIMediator WeaponInventoryUI => m_WeaponInventoryUI;

        [SerializeField]
        AttachmentItemUI[] m_AttachmentItemUIs;

        private void Start()
        {
            m_OnBeforeWeaponItemUIRemovedEvent.OnEventRaised += DetachSightFromWeaponAndResetUI;
            m_OnWeaponItemUISwappedEvent.OnEventRaised += SwapSlotIndices;
        }

        private void OnDestroy()
        {
            m_OnBeforeWeaponItemUIRemovedEvent.OnEventRaised += DetachSightFromWeaponAndResetUI;
            m_OnWeaponItemUISwappedEvent.OnEventRaised -= SwapSlotIndices;
        }

        public void AddItemToInventory(InventoryItem item)
        {
            m_Inventory.AddItemToInventory(item);
        }

        public void RemoveItemFromInventory(InventoryItem item)
        {
            m_Inventory.RemoveInventoryItem(item);
        }

        /// <summary>
        /// Drop the other SightItemUI to this SightItemUI
        /// </summary>
        /// <param name="itemUI">the other SightItemUI</param>
        public void DropSightItemUIToSlot(AttachmentItemUI itemUI, Transform slotTransform, int slotIndex)
        {
            itemUI.transform.SetParent(slotTransform);
            itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemUI.IsDragSuccess = true;

            itemUI.SetSlotIndex(slotIndex);
        }


        // ----------------------- NOTE ---------------------------------
        // WeaponItem is not necessary to be sent here, because we can get the weapon from the WeaponInventoryUI
        private void DetachSightFromWeaponAndResetUI(WeaponItem weapon, int slotIndex)
        {
            if (!TryGetSightItemUIFromSlotIndex(slotIndex, out AttachmentItemUI itemUI))
            {
                return;
            }

            itemUI.StoredItem.DetachFromWeapon();

            m_Inventory.AddItemToInventory(itemUI.StoredItem as InventoryItem);

            itemUI.ShowItemUIAndResetItsPosition();
            itemUI.ResetDataAndHideSightItemUI();
        }

        private void SwapSlotIndices(int leftIndex, int rightIndex)
        {
            if (!TryGetSightItemUIFromSlotIndex(leftIndex, out AttachmentItemUI leftUI))
            {
                Debug.LogError("This should not happen!");
                return;
            }

            if (!TryGetSightItemUIFromSlotIndex(rightIndex, out AttachmentItemUI rightUI))
            {
                Debug.LogError("This should not happen!");
                return;
            }

            leftUI.SetSlotIndex(rightIndex);
            rightUI.SetSlotIndex(leftIndex);
        }

        bool TryGetSightItemUIFromSlotIndex(int slotIndex, out AttachmentItemUI itemUI)
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
    }
}
