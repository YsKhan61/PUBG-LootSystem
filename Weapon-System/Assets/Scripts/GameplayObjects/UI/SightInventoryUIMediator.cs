
using System;
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the sight attachment UIs in the WeaponSlotUIs. [Drag drop, swap, remove, add etc]
    /// </summary>
    public class SightInventoryUIMediator : MonoBehaviour
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
        SightItemUI[] m_SightItemUIs;

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
        public void DropSightItemUIToSlot(SightItemUI itemUI, Transform slotTransform, int slotIndex)
        {
            itemUI.transform.SetParent(slotTransform);
            itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemUI.IsDragSuccess = true;

            itemUI.SetSlotIndex(slotIndex);
        }

        private void DetachSightFromWeaponAndResetUI(WeaponItem weapon, int slotIndex)
        {
            if (!TryGetSightItemUIFromSlotIndex(slotIndex, out SightItemUI itemUI))
            {
                return;
            }

            if (weapon == null || weapon.SightAttachment == null)
            {
                return;
            }

            weapon.DetachSight();

            m_Inventory.AddItemToInventory(itemUI.StoredSightItem);

            itemUI.ShowItemUIAndResetItsPosition();
            itemUI.ResetDataAndHideSightItemUI();
        }

        private void SwapSlotIndices(int leftIndex, int rightIndex)
        {
            if (TryGetSightItemUIFromSlotIndex(leftIndex, out SightItemUI itemUI))
            {
                itemUI.SetSlotIndex(rightIndex);
            }

            if (TryGetSightItemUIFromSlotIndex(rightIndex, out itemUI))
            {
                itemUI.SetSlotIndex(leftIndex);
            }
        }

        bool TryGetSightItemUIFromSlotIndex(int slotIndex, out SightItemUI itemUI)
        {
            itemUI = null;
            foreach (SightItemUI sightItemUI in m_SightItemUIs)
            {
                if (sightItemUI.SlotIndex == slotIndex)
                {
                    itemUI = sightItemUI;
                    return true;
                }
            }
            return false;
        }
    }
}
