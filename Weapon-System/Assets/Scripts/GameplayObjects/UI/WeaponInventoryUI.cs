using System;
using TMPro;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Serialization;
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

        /*[SerializeField, Tooltip("The event that will toggle the inventory")]
        BoolEventChannelSO m_ToggleInventoryEvent;*/

        /*[SerializeField, Tooltip("When a common item is added to the inventory, this event is invoked")]
        CommonItemEventChannelSO m_OnCommonItemAddedEvent;*/

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked")]
        GunItemIntEventChannelSO m_OnGunItemAddedEvent;

        [Header("Broadcast to")]

        [SerializeField]
        GunItemIntEventChannelSO m_OnGunItemRemovedEvent;

        [Space(10)]

        /*[SerializeField, Tooltip("The panel to toggle on/off")]
        GameObject m_Panel;*/

        /*[SerializeField, Tooltip("The prefab that will be spawned as child of 'm_ContentGO' when a common item is stored in inventory")]
        ItemUI m_ItemUIPrefab;

        [SerializeField, Tooltip("The spawned instance of 'm_ItemUIPrefab' will be set as a child of this transform")]
        GameObject m_ContentGO;*/

        // ItemSlotUI[] m_GunSlots;
        /*[SerializeField]
        WeaponItemUI[] m_WaponItemUIs;*/

        [SerializeField]
        WeaponItemUISlotUIPair[] m_WeaponItemUISlotUIPairs;

        private void Start()
        {
            // m_ToggleInventoryEvent.OnEventRaised += OnToggleInventory;
            // m_OnCommonItemAddedEvent.OnEventRaised += AddCommonItemUIToInventoryUI;
            m_OnGunItemAddedEvent.OnEventRaised += AddGunItemUIToInventoryUI;

            // ToggleInventoryUI(false);
        }

        private void OnDestroy()
        {
            // m_ToggleInventoryEvent.OnEventRaised -= OnToggleInventory;
            // m_OnCommonItemAddedEvent.OnEventRaised -= AddCommonItemUIToInventoryUI;
            m_OnGunItemAddedEvent.OnEventRaised -= AddGunItemUIToInventoryUI;
        }


        /// <summary>
        /// Swap the weapon item UIs in the weapon inventory UI's weapon slots.
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        public void SwapWeaponItemUIs(int leftIndex, int rightIndex)
        {
            WeaponItemUISlotUIPair leftPair = m_WeaponItemUISlotUIPairs[leftIndex];
            WeaponItemUISlotUIPair rightPair = m_WeaponItemUISlotUIPairs[rightIndex];

            // return if the weaponItemUIToDrop is the same one as in the slot
            if (leftPair.weaponItemUI == rightPair.weaponItemUI)
                return;

            // if the slot is empty, just add the weaponItemUIToDrop to the slot
            if (rightPair.weaponItemUI == null)
            {
                RemoveGunItemUIFromWeaponInventoryUI(leftPair.weaponItemUI.Item, leftIndex);
                AddGunItemUIToInventoryUI(leftPair.weaponItemUI.Item, rightIndex);
                return;
            }

            // if the slot is not empty, swap the weaponItemUIToDrop with the slot's weaponItemUI
            GunItem leftPairItem = leftPair.weaponItemUI.Item;
            GunItem rightPairItem = rightPair.weaponItemUI.Item;

            RemoveGunItemUIFromWeaponInventoryUI(leftPairItem, leftIndex);
            RemoveGunItemUIFromWeaponInventoryUI(rightPairItem, rightIndex);

            AddGunItemUIToInventoryUI(leftPairItem, rightIndex);
            AddGunItemUIToInventoryUI(rightPairItem, leftIndex);
        }

        public void RemoveGunItemUIFromWeaponInventoryUI(GunItem item, int index)
        {
            m_WeaponItemUISlotUIPairs[index].weaponSlotUI.TryRemoveItemUIFromSlotUI();
            m_OnGunItemRemovedEvent?.RaiseEvent(item, index);
        }

        public void RemoveGunItemUIFromWeaponInventoryUI(GunItem item)
        {
            RemoveGunItemUIFromWeaponInventoryUI(item, GetIndexOfWeaponItemUIFromGunItem(item));
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

        /*private void AddCommonItemUIToInventoryUI(CommonItem item)
        {
            Instantiate(m_ItemUIPrefab, m_ContentGO.transform).SetItemData(item, this);
        }*/

        private void AddGunItemUIToInventoryUI(GunItem item, int index)
        {
            WeaponItemUI weaponItemUI = m_WeaponItemUISlotUIPairs[index].weaponItemUI;
            weaponItemUI.SetItemData(item, this);
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

        /*private void OnToggleInventory(bool value)
        {
            ToggleInventoryUI(value);
        }

        private void ToggleInventoryUI(bool value)
        {
            m_Panel.SetActive(value);
        }*/
    }
}