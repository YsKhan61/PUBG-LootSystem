using System.Collections.Generic;
using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The main Inventory of the Game, where player will store all collected and storable items
    /// Only ItemUserHand is allowed to access this class.
    /// Follow FlowDiagram for more details.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        /*[Header("Listens to")]

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


        [Space(10)]*/

        [SerializeField]
        int m_SpaceAvailable = 50;

        [Header("---------Debug purposes----------")]           // For Debug purposes only

        [Header("Inventory items")]                             // For Debug purposes only

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

        [Header("Helmet")]

        [SerializeField]        // SerializeField is used only for Debug purposes
        HelmetItem m_HelmetItem;
        public HelmetItem HelmetItem => m_HelmetItem;

        [Header("Backpack")]

        [SerializeField]        // SerializeField is used only for Debug purposes
        BackpackItem m_BackpackItem;
        public BackpackItem BackpackItem => m_BackpackItem;


        private void Start()
        {
            m_InventoryItems = new List<InventoryItem>();
            m_Weapons = new WeaponItem[2];                    // For now only 2 guns are allowed
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

        public bool TryRemoveItemFromInventory(InventoryItem item)
        {
            if (!m_InventoryItems.Contains(item))
                return false;

            m_InventoryItems.Remove(item);
            m_SpaceAvailable += item.ItemData.SpaceRequired;
            Debug.Log(item.Name + " removed from inventory!");

            return true;
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

        public bool TryGetIndexOfWeaponItem(WeaponItem weaponItem, out int index)
        {
            index = -1;
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (m_Weapons[i] != weaponItem)
                    continue;

                index = i;
                return true;
            }
            return false;
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
            if (m_BackpackItem != backpackItem)
            {
                Debug.Log("Backpack not present in the inventory!");
                return false;
            }

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

        public bool TryAddHelmetToInventory(HelmetItem helmetItem)
        {
            if (m_HelmetItem != null)
            {
                Debug.Log("Helmet already present in the inventory!");
                return false;
            }

            m_HelmetItem = helmetItem;
            return true;
        }

        public bool TryRemoveHelmetFromInventory(HelmetItem helmetItem)
        {
            if (m_HelmetItem != helmetItem)
            {
                Debug.Log("Helmet not present in the inventory!");
                return false;
            }

            m_HelmetItem = null;
            return true;
        }

    }
}
