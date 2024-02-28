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
        InventoryItemEventChannelSO m_OnInventoryItemUIRemovedEvent;

        [SerializeField, Tooltip("When an WeaponItemUI is removed from WeaponInventoryUI, this event is invoked after that")]
        WeaponItemIntEventChannelSO m_OnAfterWeaponItemUIRemovedEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemUISwappedEvent;

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

        [Header("Weapon items")]

        [SerializeField, FormerlySerializedAs("m_Guns")]        // SerializeField is used only for Debug purposes
        WeaponItem[] m_Weapons;       // Only primary and secondary gun. For now only 2 guns are allowed

        private void Start()
        {
            m_InventoryItems = new List<InventoryItem>();
            m_Weapons = new WeaponItem[2];                    // For now only 2 guns are allowed

            
            m_OnInventoryItemUIRemovedEvent.OnEventRaised += RemoveInventoryItem;
            m_OnAfterWeaponItemUIRemovedEvent.OnEventRaised += RemoveGunItem;
            m_OnWeaponItemUISwappedEvent.OnEventRaised += SwapGunItems;
        }

        private void OnDestroy()
        {
            
            m_OnInventoryItemUIRemovedEvent.OnEventRaised -= RemoveInventoryItem;
            m_OnAfterWeaponItemUIRemovedEvent.OnEventRaised -= RemoveGunItem;
            m_OnWeaponItemUISwappedEvent.OnEventRaised -= SwapGunItems;
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
        public int AddGunToGunInventory(WeaponItem gunItem)
        {
            // Add to the first empty slot
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (m_Weapons[i] == null)
                {
                    m_Weapons[i] = gunItem;
                    Debug.Log(gunItem.Name + " added to inventory!");
                    return i;
                }
            }

            // If no empty slot and no gun in hand, replace the first gun
            m_Weapons[0] = gunItem;
            Debug.Log(gunItem.Name + " added to inventory!");
            return 0;
        }

        public int GetIndexOfGunItem(WeaponItem item)
        {
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (m_Weapons[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        public WeaponItem GetGunItem(int index)
        {
            if (index < 0 || index >= m_Weapons.Length)
            {
                Debug.LogError("Index out of range");
                return null;
            }

            return m_Weapons[index];
        }

        private void RemoveGunItem(WeaponItem item, int _)
        {
            int index = GetIndexOfGunItem(item);
            if (index < 0 || index >= m_Weapons.Length)
            {
                Debug.LogError("Index out of range");
                return;
            }

            if (m_Weapons[index] == null)
            {
                Debug.LogError("UI was present, but gun is not present in the inventory!");
                return;
            }

            m_Weapons[index].Drop();
            m_Weapons[index] = null;
        }

        /// <summary>
        /// Swap the gun items in the inventory
        /// And set the new gun in hand matching the previous gun's index
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        private void SwapGunItems(int leftIndex, int rightIndex)
        {
            if (leftIndex < 0 || leftIndex >= m_Weapons.Length || rightIndex < 0 || rightIndex >= m_Weapons.Length)
            {
                Debug.LogError("Index out of range");
                return;
            }

            WeaponItem temp = m_Weapons[leftIndex];
            m_Weapons[leftIndex] = m_Weapons[rightIndex];
            m_Weapons[rightIndex] = temp;
        }
    }
}
