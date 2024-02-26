#if false

using System;
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the sight attachment UIs (SightItemUIs and SightSlotUIs) in the WeaponSlotUIs.
    /// </summary>
    public class SightInventoryUI : MonoBehaviour
    {
        [Serializable]
        class SightItemUISlotUIPair
        {
            [SerializeField]
            internal SightItemUI sightItemUI;
            [SerializeField]
            internal SightSlotUI sightSlotUI;
        }


        /*[Header("Listens to")]

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked, Listen to this event to add a WeaponItemUI in respective WeaponSlotUI")]
        GunItemIntEventChannelSO m_OnGunItemAddedToInventoryEvent;*/

        [Header("Broadcast to")]

        [SerializeField, Tooltip("When an WeaponItemUI is removed from WeaponInventoryUI, this event is invoked")]
        GunItemIntEventChannelSO m_OnWeaponItemUIRemovedEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemUISwappedEvent;

        [Space(10)]

        [SerializeField]
        SightItemUISlotUIPair[] m_WeaponItemUISlotUIPairs;

        private void Start()
        {
            for (int i = 0; i < m_WeaponItemUISlotUIPairs.Length; i++)
            {
                m_WeaponItemUISlotUIPairs[i].sightItemUI.SetSlotIndex(i);
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
            SightItemUISlotUIPair leftPair = m_WeaponItemUISlotUIPairs[indexOfDroppedWeaponItemUI];
            SightItemUISlotUIPair rightPair = m_WeaponItemUISlotUIPairs[indexOfWeaponSlotUI];

            // return if the weaponItemUIToDrop is the same one as in the slot
            if (leftPair.sightItemUI == rightPair.sightItemUI)
                return;

            rightPair.sightSlotUI.TryAddItemUIToSlotUI(leftPair.sightItemUI);
            leftPair.sightSlotUI.TryAddItemUIToSlotUI(rightPair.sightItemUI);

            WeaponItemUI temp = leftPair.sightItemUI;
            leftPair.sightItemUI = rightPair.sightItemUI;
            rightPair.sightItemUI = temp;

            if (raiseEvent)
                m_OnWeaponItemUISwappedEvent?.RaiseEvent(indexOfDroppedWeaponItemUI, indexOfWeaponSlotUI);
        }

        /// <summary>
        /// Remove the GunItemUI from the WeaponInventoryUI.
        /// </summary>
        /// <param name="item">The GunItem to be removed</param>
        /// <param name="raiseEvent">True - raise an event after swap complete, False - don't raise event</param>
        public void RemoveGunItemUIFromWeaponInventoryUI(GunItem item, bool raiseEvent = true)
        {
            if (!TryGetIndexOfWeaponItemUIFromGunItem(item, out int index))
                return;

            m_WeaponItemUISlotUIPairs[index].sightSlotUI.TryRemoveItemUIFromSlotUI();

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

            gun = m_WeaponItemUISlotUIPairs[index].sightItemUI.Item;
            return gun != null;
        }

        private void AddGunItemUIToInventoryUI(GunItem item, int index)
        {
            WeaponItemUI weaponItemUI = m_WeaponItemUISlotUIPairs[index].sightItemUI;
            weaponItemUI.SetItemDataAndShow(item);
            weaponItemUI.SetSlotIndex(index);
            m_WeaponItemUISlotUIPairs[index].sightSlotUI.TryAddItemUIToSlotUI(weaponItemUI);
        }

        private bool TryGetIndexOfWeaponItemUIFromGunItem(GunItem gunItem, out int index)
        {
            index = -1;
            for (int i = 0, length = m_WeaponItemUISlotUIPairs.Length; i < length; i++)
            {
                if (m_WeaponItemUISlotUIPairs[i].sightItemUI.Item == gunItem)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }
}

#endif