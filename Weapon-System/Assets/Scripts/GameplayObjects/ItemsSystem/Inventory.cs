using System.Collections.Generic;
using UnityEngine;
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

        [SerializeField, Tooltip("To add an inventory item in the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnAddInventoryItemToInventoryEvent;

        [SerializeField, Tooltip("To remove an inventory item from inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnRemoveInventoryItemFromInventoryEvent;


        [Space(10)]

        [Header("Broadcast to")]

        [SerializeField, Tooltip("When an Inventory item is added to the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemAddedToInventory;      

        [SerializeField, Tooltip("When an Inventory item is removed from the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemRemovedFromInventory;
        

        [Space(10)]

        [SerializeField]
        int m_SpaceAvailable = 50;

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

        [SerializeField]        // SerializeField is used only for Debug purposes
        WeaponItem[] m_Weapons;       // Only primary and secondary gun. For now only 2 guns are allowed

        [Header("Backpack")]

        [SerializeField]        // SerializeField is used only for Debug purposes
        BackpackItem m_BackpackItem;
        public BackpackItem BackpackItem => m_BackpackItem;


        private void Start()
        {
            m_InventoryItems = new List<InventoryItem>();
            m_Weapons = new WeaponItem[2];                    // For now only 2 guns are allowed

            
            m_OnAddInventoryItemToInventoryEvent.OnEventRaised += AddItemToInventoryAndRaiseEvent;
            m_OnRemoveInventoryItemFromInventoryEvent.OnEventRaised += RemoveItemFromInventoryAndRaiseEvent;
        }

        private void OnDestroy()
        {
            m_OnAddInventoryItemToInventoryEvent.OnEventRaised -= AddItemToInventoryAndRaiseEvent;
            m_OnRemoveInventoryItemFromInventoryEvent.OnEventRaised -= RemoveItemFromInventoryAndRaiseEvent;
        }

        /// <summary>
        /// Later, the inventory will have a capacity, beyond which no more items can be added.
        /// That time it will return false
        /// </summary>
        /// <param name="item"></param>
        public bool TryAddItemToInventory(InventoryItem item)
        {
            int temp = m_SpaceAvailable - item.ItemData.SpaceRequired;
            if (temp <= 0)
            {
                Debug.Log("Inventory capacity exceeded!");
                return false;
            }

            m_InventoryItems.Add(item);
            m_SpaceAvailable = temp;

            Debug.Log(item.Name + " added to inventory!");
            return true;
        }

        public void AddItemToInventoryAndRaiseEvent(InventoryItem item)
        {
            TryAddItemToInventory(item);
            m_OnInventoryItemAddedToInventory.RaiseEvent(item);
        }

        public bool TryRemoveItemFromInventory(InventoryItem item)
        {
            if (!m_InventoryItems.Contains(item))
                return false;

            m_InventoryItems.Remove(item);
            m_SpaceAvailable += item.ItemData.SpaceRequired;
            Debug.Log(item.Name + " removed from inventory!");

            return true;
        }

        public void RemoveItemFromInventoryAndRaiseEvent(InventoryItem item)
        {
            TryRemoveItemFromInventory(item);
            m_OnInventoryItemRemovedFromInventory.RaiseEvent(item);
        }

        /// <summary>
        /// Stores the gun in separate array.
        /// </summary>
        /// <param name="weaponItem">The gun item to store</param>
        public void AddWeaponItemToWeaponInventory(WeaponItem weaponItem)
        {
            // Try add to the first empty slot
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (m_Weapons[i] != null)
                    continue;

                AddWeaponItemToStorageIndex(weaponItem, i);
                return;
            }

            // Try replacing the gun in hand with the new gun
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (!m_Weapons[i].IsInHand)
                    continue;

                TryRemoveWeaponItem(m_Weapons[i]);

                AddWeaponItemToStorageIndex(weaponItem, i);
                return;
            }

            // If no empty slot is present, nor is there any gun in hand,
            // replace the first gun with the new gun.
            TryRemoveWeaponItem(m_Weapons[0]);

            // Store the new gun in the empty slot
            AddWeaponItemToStorageIndex(weaponItem, 0);
        }

        public bool AddWeaponItemToStorageIndex(in WeaponItem weaponItem, in int index)
        {
            if (TryGetWeaponItem(index, out WeaponItem _))
            {
                Debug.LogError("Gun already present in the inventory at index: " + index);
                return false;
            }
            // Store the new gun in the empty slot
            m_Weapons[index] = weaponItem;
            Debug.Log(weaponItem.Name + " added to inventory!");

            return true;
            // m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent.RaiseEvent(weaponItem, index);
        }

        public bool TryRemoveWeaponItemFromStorage(in int index)
        {
            if (m_Weapons[index] == null)
            {
                Debug.LogError("No gun present in the inventory at index: " + index);
                return false;
            }

            m_Weapons[index] = null;
            return true;
        }

        /// <summary>
        /// Swap the Weapon items in the inventory
        /// And set the new gun in hand matching the previous gun's index
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        public bool TrySwapWeaponItemsInWeaponStorage(int leftIndex, int rightIndex)
        {
            if (leftIndex < 0 || leftIndex >= m_Weapons.Length || rightIndex < 0 || rightIndex >= m_Weapons.Length)
            {
                Debug.LogError("Index out of range");
                return false;
            }

            WeaponItem temp = null;

            if (m_Weapons[leftIndex] != null)
            {
                temp = m_Weapons[leftIndex];
            }
            if (m_Weapons[rightIndex] != null)
            {
                m_Weapons[leftIndex] = m_Weapons[rightIndex];
            }

            m_Weapons[rightIndex] = temp;

            // m_OnWeaponItemSwappedInInventoryEvent.RaiseEvent(leftIndex, rightIndex);
            return true;
        }

        public bool TryGetEmptyWeaponStorageIndex(out int index)
        {
            index = -1;
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (m_Weapons[i] != null)
                    continue;

                index = i;
                return true;
            }
            return false;
        }

        public bool TryGetIndexOfWeaponInHand(out int index)
        {
            index = -1;
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (!m_Weapons[i].IsInHand)
                    continue;
                
                index = i;
                return true;
            }
            return false;
        }

        public bool TryGetWeaponItem(int index, out WeaponItem weaponItem)
        {
            weaponItem = null;
            if (index < 0 || index >= m_Weapons.Length)
            {
                Debug.LogError("Index out of range");
                return false;
            }

            if (m_Weapons[index] == null)
            {
                return false;
            }

            weaponItem = m_Weapons[index];
            return true;
        }

        public bool TryAddBackpackToInventory(BackpackItem backpackItem)
        {
            if (m_BackpackItem != null)
            {
                Debug.Log("Backpack already present in the inventory!");
                return false;
            }

            m_SpaceAvailable -= backpackItem.ItemData.SpaceRequired;       // Here backpackItem.ItemData.SpaceRequired will be provided as negative value
            m_BackpackItem = backpackItem;
            return true;
        }

        public bool TryRemoveBackpackFromInventory(BackpackItem backpackItem)
        {
            int temp = m_SpaceAvailable + backpackItem.ItemData.SpaceRequired;       // Here backpackItem.ItemData.SpaceRequired will be provided as positive value
            if (temp < 0)
            {
                Debug.Log("Can't remove backpack. Items will overflow!");
                return false;
            }

            m_SpaceAvailable = temp;
            m_BackpackItem = null;
            return true;
        }

        private void OnAddWeaponItemToWeaponInventoryToSpecificIndex(WeaponItem weaponItem, int index)
        {
            // Now add the new weapon item to the index
            AddWeaponItemToStorageIndex(weaponItem, index);
        }

        private void OnRemoveWeaponItemFromWeaponInventoryFromSpecificIndex(WeaponItem weaponItem, int index)
        {
            TryRemoveWeaponItem(weaponItem);
        }

        private bool TryRemoveWeaponItem(in WeaponItem weaponItem)
        {
            if (!TryGetIndexOfWeaopnItem(weaponItem, out int index))
                return false;

            // m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent.RaiseEvent(weaponItem, index);
            // m_OnWeaponItemRemovedFromWeaponInventoryFromSpecificIndexEvent.RaiseEvent(weaponItem, index);
            m_Weapons[index] = null;
            return true;
        }

        private bool TryGetIndexOfWeaopnItem(WeaponItem item, out int index)
        {
            index = -1;
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (m_Weapons[i] == item)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }
    }
}
