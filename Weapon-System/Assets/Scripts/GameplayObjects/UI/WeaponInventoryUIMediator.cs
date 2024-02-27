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
        [Serializable]
        class WeaponItemUISlotUIPair
        {
            [SerializeField]
            internal WeaponItemUI weaponItemUI;
            [SerializeField]
            internal WeaponSlotUI weaponSlotUI;
        }


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
        WeaponItemUISlotUIPair[] m_WeaponItemUISlotUIPairs;

        private void Start()
        {
            for (int i = 0; i < m_WeaponItemUISlotUIPairs.Length; i++)
            {
                m_WeaponItemUISlotUIPairs[i].weaponItemUI.SetSlotIndex(i);
            }

            m_OnGunItemAddedToInventoryEvent.OnEventRaised += AddGunItemUIToInventoryUI;
        }

        private void OnDestroy()
        {
            m_OnGunItemAddedToInventoryEvent.OnEventRaised -= AddGunItemUIToInventoryUI;
        }


        /// <summary>
        /// Swap the weapon item UIs in the weapon inventory UI's weapon slots.
        /// </summary>
        /// <param name="indexOfDroppedWeaponItemUI">index of the WeaponItemUI that is being dropped</param>
        /// <param name="indexOfWeaponSlotUI">index of the WeaponSlotUI where the WeaponItemUI is being dropped</param>
        /// <param name="raiseEvent">True - raise an event after swap complete, False - don't raise event</param>
        /// <remarks>
        /// It would be more simpler if we had have only 2 slots,
        /// but if we want more slots, and swap UIs between them,
        /// then this approach is better.
        /// </remarks>
        public void SwapWeaponItemUIs(int indexOfDroppedWeaponItemUI, int indexOfWeaponSlotUI, bool raiseEvent = true)
        {
            WeaponItemUISlotUIPair leftPair = m_WeaponItemUISlotUIPairs[indexOfDroppedWeaponItemUI];
            WeaponItemUISlotUIPair rightPair = m_WeaponItemUISlotUIPairs[indexOfWeaponSlotUI];

            // return if the weaponItemUIToDrop is the same one as in the slot
            if (leftPair.weaponItemUI == rightPair.weaponItemUI)
                return;

            rightPair.weaponSlotUI.TryAddItemUIToSlotUI(leftPair.weaponItemUI);
            leftPair.weaponSlotUI.TryAddItemUIToSlotUI(rightPair.weaponItemUI);

            WeaponItemUI temp = leftPair.weaponItemUI;
            leftPair.weaponItemUI = rightPair.weaponItemUI;
            rightPair.weaponItemUI = temp;

            if (raiseEvent)
                m_OnWeaponItemUISwappedEvent?.RaiseEvent(indexOfDroppedWeaponItemUI, indexOfWeaponSlotUI);
        }

        /// <summary>
        /// Remove the GunItemUI from the GunSlotUI and raise an event.
        /// </summary>
        /// <param name="item">The GunItem to be removed</param>
        /// <param name="raiseEvent">True - raise an event after swap complete, False - don't raise event</param>
        public void RemoveGunItemUIFromGunSlotUI(GunItem item, bool raiseEvent = true)
        {
            if (!TryGetIndexOfWeaponItemUIFromGunItem(item, out int index))
                return;

            m_WeaponItemUISlotUIPairs[index].weaponSlotUI.TryRemoveItemUIFromSlotUI();

            if (raiseEvent)
                m_OnWeaponItemUIRemovedEvent?.RaiseEvent(item, index);
        }

        public bool TryGetGunItemFromWeaponInventoryUI(int index, out GunItem gun)
        {
            gun = null;

            if (index < 0 || index >= m_WeaponItemUISlotUIPairs.Length)
            {
                Debug.LogError("Index out of range");
                return false;
            }

            gun = m_WeaponItemUISlotUIPairs[index].weaponItemUI.GunItem;
            return gun != null;
        }

        private void AddGunItemUIToInventoryUI(GunItem item, int index)
        {
            WeaponItemUI weaponItemUI = m_WeaponItemUISlotUIPairs[index].weaponItemUI;
            weaponItemUI.SetItemDataAndShow(item);
            weaponItemUI.SetSlotIndex(index);
            m_WeaponItemUISlotUIPairs[index].weaponSlotUI.TryAddItemUIToSlotUI(weaponItemUI);
        }

        private bool TryGetIndexOfWeaponItemUIFromGunItem(GunItem gunItem, out int index)
        {
            index = -1;
            for (int i = 0, length = m_WeaponItemUISlotUIPairs.Length; i < length; i++)
            {
                if (m_WeaponItemUISlotUIPairs[i].weaponItemUI.GunItem == gunItem)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }
}