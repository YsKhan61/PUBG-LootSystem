using System;
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the Weapon related UIs (WeaponItemUIs and WeaponSlotUIs) in the WeaponSlotUIs.
    /// </summary>
    public class WeaponInventoryUIMediator : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked, Listen to this event to add a WeaponItemUI in respective WeaponSlotUI")]
        GunItemIntEventChannelSO m_OnGunItemAddedToInventoryEvent;

        [Header("Broadcast to")]

        [SerializeField, Tooltip("When an WeaponItemUI is removed from WeaponInventoryUI, this event is invoked")]
        GunItemIntEventChannelSO m_OnWeaponItemUIRemovedEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemUISwappedEvent;

        [Space(10)]

        [SerializeField]
        WeaponItemUI[] m_WeaponItemUIs;

        [SerializeField]
        Inventory m_Inventory;

        private void Start()
        {
            m_OnGunItemAddedToInventoryEvent.OnEventRaised += AddGunItemUIToInventoryUI;
        }

        private void OnDestroy()
        {
            m_OnGunItemAddedToInventoryEvent.OnEventRaised -= AddGunItemUIToInventoryUI;
        }

        public void BroadcastWeaponItemUIRemovedEvent(GunItem item, int slotIndex)
        {
            m_OnWeaponItemUIRemovedEvent?.RaiseEvent(item, slotIndex);
        }

        public void BroadcastWeaponItemUIsSwappedEvent(int indexOfDroppedWeaponItemUI, int indexOfWeaponSlotUI)
        {
            m_OnWeaponItemUISwappedEvent?.RaiseEvent(indexOfDroppedWeaponItemUI, indexOfWeaponSlotUI);
        }

        public bool TryGetGunItemFromWeaponInventoryUI(int index, out GunItem gun)
        {
            gun = null;

            if (index < 0 || index >= m_WeaponItemUIs.Length)
            {
                Debug.LogError("Index out of range");
                return false;
            }

            gun = m_WeaponItemUIs[index].StoredGunItem;
            return gun != null;
        }

        public void DropWeaponItemUIToSlot(WeaponItemUI itemUI, Transform slotTransform, int slotIndex)
        {
            itemUI.transform.SetParent(slotTransform);
            itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemUI.IsDragSuccess = true;

            itemUI.SetSlotIndex(slotIndex);
        }

        private void AddGunItemUIToInventoryUI(GunItem item, int index)
        {
            // If the gun is already in the inventory, then return for now.
            // This is a temporary solution, as we are not removing the gun from the inventory.
            // We are just adding the gun to the inventory.
            // Later, we gonna implement the mechanics applied in PUBG
            if (TryGetGunItemFromWeaponInventoryUI(index, out GunItem _))
            {
                return;
            }

            if (!TryGetWeaponItemUIFromSlotIndex(index, out WeaponItemUI weaponItemUI))
            {
                Debug.LogError("WeaponItemUI not found for the index: " + index);
                return;
            }

            weaponItemUI.SetItemDataAndShow(item);
        }

        private bool TryGetWeaponItemUIFromSlotIndex(int index, out WeaponItemUI itemUI)
        {
            itemUI = null;
            foreach (WeaponItemUI weaponItemUI in m_WeaponItemUIs)
            {
                if (weaponItemUI.SlotIndex == index)
                {
                    itemUI = weaponItemUI;
                    return true;
                }
            }

            return false;
        }
    }
}