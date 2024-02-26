using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  Need to create an HashSet of all the items that will be stored in the inventory
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [Header("Broadcast to")]

        [SerializeField, Tooltip("When a common item is added to the inventory, this event is invoked.")]
        [FormerlySerializedAs("m_OnCommonItemAddedEvent")]
        InventoryItemEventChannelSO m_InventoryItemAddedEvent;

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked.")]
        GunItemIntEventChannelSO m_OnGunItemAddedEvent;

        [Header("Listens to")]

        [SerializeField, Tooltip("Listen to this event to remove the respective common item from inventory.")]
        [FormerlySerializedAs("m_OnCommonItemUIRemovedEvent")]
        InventoryItemEventChannelSO m_OnInventoryItemUIRemovedEvent;

        [SerializeField, Tooltip("Listen to this event to remove the respective gun item from inventory.")]
        GunItemIntEventChannelSO m_OnGunItemUIRemovedEvent;

        [SerializeField, Tooltip("Listen to this event to swap the respective guns in inventory.")]
        IntIntEventChannelSO m_OnGunItemUISwappedEvent;

        [Space(10)]

        /*[SerializeField]
        ItemUserHand m_UserHand;*/

        [Space(10)]

        [Header("---------Debug purposes----------")]

        [Header("Inventory items")]


        /// <summary>
        /// We are not using IStorable interface rather using InventoryItem,
        /// because we might need informations like Name, Description, ItemDataSO, etc. of the item
        /// </summary>
        [SerializeField]        // SerializeField is used only for Debug purposes
        List<InventoryItem> m_InventoryItems;

        [Space(10)]

        [Header("Gun items")]

        [SerializeField]        // SerializeField is used only for Debug purposes
        GunItem[] m_Guns;       // Only primary and secondary gun. For now only 2 guns are allowed

        private void Start()
        {
            m_InventoryItems = new List<InventoryItem>();
            m_Guns = new GunItem[2];                    // For now only 2 guns are allowed

            
            m_OnInventoryItemUIRemovedEvent.OnEventRaised += RemoveInventoryItem;
            m_OnGunItemUIRemovedEvent.OnEventRaised += RemoveGunItem;
            m_OnGunItemUISwappedEvent.OnEventRaised += SwapGunItems;
        }

        private void OnDestroy()
        {
            
            m_OnInventoryItemUIRemovedEvent.OnEventRaised -= RemoveInventoryItem;
            m_OnGunItemUIRemovedEvent.OnEventRaised -= RemoveGunItem;
            m_OnGunItemUISwappedEvent.OnEventRaised -= SwapGunItems;
        }

        public void AddItemToInventory(InventoryItem item)
        {
            m_InventoryItems.Add(item);
            m_InventoryItemAddedEvent.RaiseEvent(item);
            Debug.Log(item.Name + " added to inventory!");
        }

        public void RemoveInventoryItem(InventoryItem item)
        {
            m_InventoryItems.Remove(item);
            Debug.Log(item.Name + " removed from inventory!");
        }

        public void AddGunToGunInventory(GunItem gunItem)
        {
            // Add to the first empty slot
            for (int i = 0; i < m_Guns.Length; i++)
            {
                if (m_Guns[i] == null)
                {
                    m_Guns[i] = gunItem;
                    // m_UserHand.ItemInHand = item;                  // Set the gun in hand
                    m_OnGunItemAddedEvent.RaiseEvent(gunItem, i);
                    Debug.Log(gunItem.Name + " added to inventory!");
                    return;
                }
            }

            // If no empty slot and no gun in hand, replace the first gun
            m_Guns[0] = gunItem;
            // m_UserHand.ItemInHand = gunItem;                  // Set the gun in hand
            m_OnGunItemAddedEvent.RaiseEvent(gunItem, 0);
            Debug.Log(gunItem.Name + " added to inventory!");
        }

        public int GetIndexOfGunItem(GunItem item)
        {
            for (int i = 0; i < m_Guns.Length; i++)
            {
                if (m_Guns[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        public GunItem GetGunItem(int index)
        {
            if (index < 0 || index >= m_Guns.Length)
            {
                Debug.LogError("Index out of range");
                return null;
            }

            return m_Guns[index];
        }

        private void RemoveGunItem(GunItem item, int _)
        {
            int index = GetIndexOfGunItem(item);
            if (index < 0 || index >= m_Guns.Length)
            {
                Debug.LogError("Index out of range");
                return;
            }

            if (m_Guns[index] == null)
            {
                Debug.LogError("UI was present, but gun is not present in the inventory!");
                return;
            }

            m_Guns[index].Drop();
            m_Guns[index] = null;
        }

        /// <summary>
        /// Swap the gun items in the inventory
        /// And set the new gun in hand matching the previous gun's index
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        private void SwapGunItems(int leftIndex, int rightIndex)
        {
            if (leftIndex < 0 || leftIndex >= m_Guns.Length || rightIndex < 0 || rightIndex >= m_Guns.Length)
            {
                Debug.LogError("Index out of range");
                return;
            }

            GunItem temp = m_Guns[leftIndex];
            m_Guns[leftIndex] = m_Guns[rightIndex];
            m_Guns[rightIndex] = temp;
        }
    }
}
