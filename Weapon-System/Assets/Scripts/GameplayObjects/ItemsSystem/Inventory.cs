using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The main Inventory of the Game, where player will store all collected and storable items
    /// This will be only listening to events, and will not be broadcasting any event.
    /// It will listen to directly collected item event, or when event is raised by ItemUI added to the InventoryUI
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("Listen to this event to remove the respective common item from inventory.")]
        [FormerlySerializedAs("m_OnCommonItemUIRemovedEvent")]
        InventoryItemEventChannelSO m_OnInventoryItemUIRemovedEvent;

        [SerializeField, Tooltip("Listen to this event to remove the respective gun item from inventory.")]
        GunItemIntEventChannelSO m_OnGunItemUIRemovedEvent;

        [SerializeField, Tooltip("Listen to this event to swap the respective guns in inventory.")]
        IntIntEventChannelSO m_OnGunItemUISwappedEvent;

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
            Debug.Log(item.Name + " added to inventory!");
        }

        public void RemoveInventoryItem(InventoryItem item)
        {
            m_InventoryItems.Remove(item);
            Debug.Log(item.Name + " removed from inventory!");
        }

        /// <summary>
        /// Stores the gun item in separate array.
        /// </summary>
        /// <param name="gunItem">The gun item to store</param>
        /// <returns>Returns the index of the GunItem array where the gun is stored</returns>
        public int AddGunToGunInventory(GunItem gunItem)
        {
            // Add to the first empty slot
            for (int i = 0; i < m_Guns.Length; i++)
            {
                if (m_Guns[i] == null)
                {
                    m_Guns[i] = gunItem;
                    Debug.Log(gunItem.Name + " added to inventory!");
                    return i;
                }
            }

            // If no empty slot and no gun in hand, replace the first gun
            m_Guns[0] = gunItem;
            Debug.Log(gunItem.Name + " added to inventory!");
            return 0;
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
