using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// The UI of the inventory that will showcase all the picked up items.
    /// It is the main UI of the game scene. 
    /// It will be toggled on/off by the player.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("When an Inventory item is added to the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemAddedEvent;

        [Space(10)]

        [Header("Broadcast to")]

        [SerializeField, Tooltip("When an Inventory Item UI is added to the InventoryUI, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemUIAddedEvent;

        [SerializeField, Tooltip("When an Inventory item UI is removed from the inventory UI, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemUIRemovedEvent;

        [Space(10)]

        [SerializeField, Tooltip("The prefab that will be spawned as child of 'm_ContentGO' when a common item is stored in inventory")]
        ItemUI m_ItemUIPrefab;

        [SerializeField, Tooltip("The spawned instance of 'm_ItemUIPrefab' will be set as a child of this transform")]
        GameObject m_ContentGO;


        private void Start()
        {
            m_OnInventoryItemAddedEvent.OnEventRaised += CreateInventoryItemUI;
        }

        private void OnDestroy()
        {
            m_OnInventoryItemAddedEvent.OnEventRaised -= CreateInventoryItemUI;
        }

        public void RaiseOnInventoryItemUIAddedEvent(InventoryItem item)
        {
            m_OnInventoryItemUIAddedEvent.RaiseEvent(item);
        }

        public void RaiseOnInventoryItemUIRemovedEvent(InventoryItem item)
        {
            m_OnInventoryItemUIRemovedEvent.RaiseEvent(item);
        }

        private void CreateInventoryItemUI(InventoryItem item)
        {
            Instantiate(m_ItemUIPrefab, m_ContentGO.transform).SetItemData(item, this);
        }
    }
}