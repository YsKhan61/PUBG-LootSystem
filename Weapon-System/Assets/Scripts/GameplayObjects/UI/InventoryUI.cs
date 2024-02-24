using TMPro;
using UnityEngine;
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

        [SerializeField]
        BoolEventChannelSO m_ToggleInventoryEvent;

        [SerializeField]
        ItemDataEventChannelSO m_OnItemAddedEvent;

        [Space(10)]

        [SerializeField]
        GameObject m_Panel;

        [SerializeField]
        ItemUI m_ItemUIPrefab;

        [SerializeField]
        GameObject m_ContentGO;

        [SerializeField]
        ItemSlotUI[] m_WeaponSlots;

        private void Start()
        {
            m_ToggleInventoryEvent.OnEventRaised += OnToggleInventory;
            m_OnItemAddedEvent.OnEventRaised += AddItemUIToInventoryUI;

            ToggleInventoryUI(false);
        }

        private void OnDestroy()
        {
            m_ToggleInventoryEvent.OnEventRaised -= OnToggleInventory;
            m_OnItemAddedEvent.OnEventRaised -= AddItemUIToInventoryUI;
        }

        public void AddItemUIToInventoryUI(ItemDataSO itemData)
        {
            switch (itemData.UIType)
            { 
                case ItemUIType.Common:
                    AddCommonItemUIToInventoryUI(itemData);
                    break;
                case ItemUIType.Gun:
                    AddGunItemUIToInventoryUI(itemData);
                    break;
                case ItemUIType.ScopeAttachment:
                    AddScopeItemUIToInventoryUI(itemData);
                    break;
            }
        }

        public void RemoveItemUIFromInventory()
        {
            
        }

        private void AddCommonItemUIToInventoryUI(ItemDataSO itemData)
        {
            Instantiate(m_ItemUIPrefab, m_ContentGO.transform).SetItemData(itemData);
        }

        private void AddGunItemUIToInventoryUI(ItemDataSO itemData)
        {
            foreach (ItemSlotUI slot in m_WeaponSlots)
            {
                if (slot.IsHavingItem)
                {
                    continue;
                }

                slot.TryAddItemToSlotUI(itemData);
            }
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