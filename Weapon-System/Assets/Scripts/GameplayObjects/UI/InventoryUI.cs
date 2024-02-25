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
    public class InventoryUI : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("The event that will toggle the inventory")]
        BoolEventChannelSO m_ToggleInventoryEvent;

        [SerializeField, Tooltip("When a common item is added to the inventory, this event is invoked")]
        CommonItemEventChannelSO m_OnCommonItemAddedEvent;

        /*[SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked")]
        GunItemIntEventChannelSO m_OnGunItemAddedEvent;*/

        [Header("Broadcast to")]

        [SerializeField]
        CommonItemEventChannelSO m_OnCommonItemRemovedEvent;

        [Space(10)]

        [SerializeField, Tooltip("The panel to toggle on/off")]
        GameObject m_Panel;

        [SerializeField, Tooltip("The prefab that will be spawned as child of 'm_ContentGO' when a common item is stored in inventory")]
        ItemUI m_ItemUIPrefab;

        [SerializeField, Tooltip("The spawned instance of 'm_ItemUIPrefab' will be set as a child of this transform")]
        GameObject m_ContentGO;

        /*ItemSlotUI[] m_GunSlots;*/

        private void Start()
        {
            m_ToggleInventoryEvent.OnEventRaised += OnToggleInventory;
            m_OnCommonItemAddedEvent.OnEventRaised += AddCommonItemUIToInventoryUI;
            // m_OnGunItemAddedEvent.OnEventRaised += AddGunItemUIToInventoryUI;

            ToggleInventoryUI(false);
        }

        private void OnDestroy()
        {
            m_ToggleInventoryEvent.OnEventRaised -= OnToggleInventory;
            m_OnCommonItemAddedEvent.OnEventRaised -= AddCommonItemUIToInventoryUI;
            // m_OnGunItemAddedEvent.OnEventRaised -= AddGunItemUIToInventoryUI;
        }

        public void RemoveCommonItemUIFromInventoryUI(CommonItem item)
        {
            m_OnCommonItemRemovedEvent?.RaiseEvent(item);
        }

        private void AddCommonItemUIToInventoryUI(CommonItem item)
        {
            Instantiate(m_ItemUIPrefab, m_ContentGO.transform).SetItemData(item, this);
        }

        /*private void AddGunItemUIToInventoryUI(ItemBase item, int index)
        {
            m_GunSlots[index].TryAddItemToSlotUI(item.ItemData);
        }*/

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

        private void OnToggleInventory(bool value)
        {
            ToggleInventoryUI(value);
        }

        private void ToggleInventoryUI(bool value)
        {
            m_Panel.SetActive(value);
        }
    }
}