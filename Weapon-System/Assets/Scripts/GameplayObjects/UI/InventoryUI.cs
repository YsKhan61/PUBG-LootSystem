using TMPro;
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
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

        private void Start()
        {
            m_ToggleInventoryEvent.OnEventRaised += OnToggleInventory;
            m_OnItemAddedEvent.OnEventRaised += AddItemUIToInventory;

            ToggleInventoryUI(false);
        }

        private void OnDestroy()
        {
            m_ToggleInventoryEvent.OnEventRaised -= OnToggleInventory;
            m_OnItemAddedEvent.OnEventRaised -= AddItemUIToInventory;
        }

        public void AddItemUIToInventory(ItemDataSO itemData)
        {
            Instantiate(m_ItemUIPrefab, m_ContentGO.transform).SetItemData(itemData);
        }

        public void RemoveItemUIFromInventory()
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