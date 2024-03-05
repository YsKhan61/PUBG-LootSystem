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

        [SerializeField, Tooltip("To add an weapon item in the inventory without any specific slot, this event is invoked")]
        WeaponItemEventChannelSO m_OnAddWeaponItemToWeaponInventoryEvent;

        [SerializeField, Tooltip("To add an weapon item in the weapon inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnAddWeaponItemToWeaponInventoryToSpecificIndexEvent;

        [SerializeField, Tooltip("To remove an weapon item from weapon inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnRemoveWeaponItemFromWeaponInventoryFromSpecificIndexEvent;

        [SerializeField, Tooltip("To swap two WeaponItemUI's in the inventory, this event is invoked")]
        IntIntEventChannelSO m_OnSwapWeaponItemUIsInInventoryEvent;


        [Space(10)]

        [Header("Broadcast to")]

        [SerializeField, Tooltip("When an Inventory item is added to the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemAddedToInventory;      

        [SerializeField, Tooltip("When an Inventory item is removed from the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemRemovedFromInventory;


        [SerializeField, Tooltip("When an Weapon item is added to the inventory to specific index, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent;

        [SerializeField, Tooltip("Before an Weaopn item is removed from the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent;

        [SerializeField, Tooltip("When an Weapon item is removed from the inventory from specific index, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemRemovedFromWeaponInventoryFromSpecificIndexEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other in the inventory, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemSwappedInInventoryEvent;

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


        private void Start()
        {
            m_InventoryItems = new List<InventoryItem>();
            m_Weapons = new WeaponItem[2];                    // For now only 2 guns are allowed

            
            m_OnAddInventoryItemToInventoryEvent.OnEventRaised += AddItemToInventoryAndRaiseEvent;
            m_OnRemoveInventoryItemFromInventoryEvent.OnEventRaised += RemoveItemFromInventoryAndRaiseEvent;

            m_OnAddWeaponItemToWeaponInventoryEvent.OnEventRaised += AddWeaponItemToWeaponInventory;
            m_OnAddWeaponItemToWeaponInventoryToSpecificIndexEvent.OnEventRaised += OnAddWeaponItemToWeaponInventoryToSpecificIndex;
            m_OnRemoveWeaponItemFromWeaponInventoryFromSpecificIndexEvent.OnEventRaised += OnRemoveWeaponItemFromWeaponInventoryFromSpecificIndex;
            m_OnSwapWeaponItemUIsInInventoryEvent.OnEventRaised += SwapWeaponItems;
        }

        private void OnDestroy()
        {
            m_OnAddInventoryItemToInventoryEvent.OnEventRaised -= AddItemToInventoryAndRaiseEvent;
            m_OnRemoveInventoryItemFromInventoryEvent.OnEventRaised -= RemoveItemFromInventoryAndRaiseEvent;

            m_OnAddWeaponItemToWeaponInventoryEvent.OnEventRaised -= AddWeaponItemToWeaponInventory;
            m_OnAddWeaponItemToWeaponInventoryToSpecificIndexEvent.OnEventRaised -= OnAddWeaponItemToWeaponInventoryToSpecificIndex;
            m_OnRemoveWeaponItemFromWeaponInventoryFromSpecificIndexEvent.OnEventRaised -= OnRemoveWeaponItemFromWeaponInventoryFromSpecificIndex;
            m_OnSwapWeaponItemUIsInInventoryEvent.OnEventRaised -= SwapWeaponItems;
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

                AddWeaponItemToIndex(weaponItem, i);
                return;
            }

            // Try replacing the gun in hand with the new gun
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                if (!m_Weapons[i].IsInHand)
                    continue;

                TryRemoveWeaponItem(m_Weapons[i]);

                AddWeaponItemToIndex(weaponItem, i);
                return;
            }

            // If no empty slot is present, nor is there any gun in hand,
            // replace the first gun with the new gun.
            TryRemoveWeaponItem(m_Weapons[0]);

            // Store the new gun in the empty slot
            AddWeaponItemToIndex(weaponItem, 0);
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
                // For now we are not swapping backpacks, so return false
                // TODO - Later we need to swap backpacks

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
            AddWeaponItemToIndex(weaponItem, index);
        }

        private void OnRemoveWeaponItemFromWeaponInventoryFromSpecificIndex(WeaponItem weaponItem, int index)
        {
            TryRemoveWeaponItem(weaponItem);
        }

        /// <summary>
        /// Swap the Weapon items in the inventory
        /// And set the new gun in hand matching the previous gun's index
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="rightIndex"></param>
        private void SwapWeaponItems(int leftIndex, int rightIndex)
        {
            if (leftIndex < 0 || leftIndex >= m_Weapons.Length || rightIndex < 0 || rightIndex >= m_Weapons.Length)
            {
                Debug.LogError("Index out of range");
                return;
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

            m_OnWeaponItemSwappedInInventoryEvent.RaiseEvent(leftIndex, rightIndex);
        }

        private void AddWeaponItemToIndex(WeaponItem weaponItem, int index)
        {
            if (TryGetWeaponItem(index, out WeaponItem _))
            {
                Debug.LogError("Gun already present in the inventory at index: " + index);
                return;
            }
            // Store the new gun in the empty slot
            m_Weapons[index] = weaponItem;
            Debug.Log(weaponItem.Name + " added to inventory!");

            m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent.RaiseEvent(weaponItem, index);
        }

        private void TryRemoveWeaponItem(in WeaponItem weaponItem)
        {
            if (!TryGetIndexOfWeaopnItem(weaponItem, out int index))
                return;

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
            m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent.RaiseEvent(weaponItem, index);
            m_OnWeaponItemRemovedFromWeaponInventoryFromSpecificIndexEvent.RaiseEvent(weaponItem, index);
            m_Weapons[index] = null;
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
