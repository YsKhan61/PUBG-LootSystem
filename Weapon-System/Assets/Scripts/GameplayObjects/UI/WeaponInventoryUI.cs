using System;
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// The UI of the inventory that will showcase all the picked up items.
    /// </summary>
    public class WeaponInventoryUI : MonoBehaviour
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
        /// <remarks>
        /// It would be more simpler if we had have only 2 slots,
        /// but if we want more slots, and swap UIs between them,
        /// then this approach is better.
        /// </remarks>
        public void SwapWeaponItemUIs(int indexOfDroppedWeaponItemUI, int indexOfWeaponSlotUI)
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

            m_OnWeaponItemUISwappedEvent?.RaiseEvent(indexOfDroppedWeaponItemUI, indexOfWeaponSlotUI);
        }

        public void RemoveGunItemUIFromWeaponInventoryUI(GunItem item)
        {
            int index = GetIndexOfWeaponItemUIFromGunItem(item);
            // if the item is not in the inventory, return
            if (index < 0 && index >= m_WeaponItemUISlotUIPairs.Length)
                return;

            m_WeaponItemUISlotUIPairs[index].weaponSlotUI.TryRemoveItemUIFromSlotUI();
            m_OnWeaponItemUIRemovedEvent?.RaiseEvent(item, index);
        }

        public WeaponSlotUI GetWeaponSlotUI(WeaponItemUI itemUI)
        {
            return m_WeaponItemUISlotUIPairs[GetIndexOfWeaponItemUI(itemUI)].weaponSlotUI;
        }

        public int GetIndexOfWeaponItemUI(WeaponItemUI weaponItemUI)
        {
            return GetIndexOfWeaponItemUIFromGunItem(weaponItemUI.Item as GunItem);
        }

        public int GetIndexOfWeaponItemUIFromGunItem(GunItem gunItem)
        {
            for (int i = 0, length = m_WeaponItemUISlotUIPairs.Length; i < length; i++)
            {
                if (m_WeaponItemUISlotUIPairs[i].weaponItemUI.Item == gunItem)
                {
                    return i;
                }
            }

            return -1;
        }

        private void AddGunItemUIToInventoryUI(GunItem item, int index)
        {
            WeaponItemUI weaponItemUI = m_WeaponItemUISlotUIPairs[index].weaponItemUI;
            weaponItemUI.SetItemData(item);
            weaponItemUI.SetSlotIndex(index);
            m_WeaponItemUISlotUIPairs[index].weaponSlotUI.TryAddItemUIToSlotUI(weaponItemUI);
        }

        /// <summary>
        /// For scopes, after pickup, we will check if any gun slot has a gun,
        /// if yes, we will check if it's scope slot is empty,
        /// if yes, we will add the pickedup scope to that empty slot,
        /// other wise, we will do AddCommonItemUIToInventoryUI(itemData);
        /// </summary>
        /// <param name="itemData"></param>
        private void AddScopeItemUIToInventoryUI(ItemDataSO itemData)
        {
            
        }
    }
}