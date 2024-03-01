using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


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

        [SerializeField, Tooltip("When Toggle Inventory UI input is performed, this event is invoked")]
        BoolEventChannelSO m_OnToggleInventoryUIEvent;

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

        [SerializeField, Tooltip("To show ItemUI in Viscinity, The spawned instance of 'm_ItemUIPrefab' will be set as a child of this transform")]
        GameObject m_ViscinityContentGO;

        [SerializeField, Tooltip("To show ItemUI in Inventory, The spawned instance of 'm_ItemUIPrefab' will be set as a child of this transform")]
        GameObject m_InventoryContentGO;

        [SerializeField]
        ItemUserHand m_ItemUserHand;

        [SerializeField]
        PoolManager m_PoolManager;

        [SerializeField]
        Canvas m_Canvas;
        public Transform CanvasTransform => m_Canvas.transform;

        public float CanvasScaleFactor => m_Canvas.scaleFactor;

        List<ItemUI> m_ItemUIs;


        private void Start()
        {
            m_ItemUIs = new List<ItemUI>();
            // m_OnInventoryItemAddedEvent.OnEventRaised += CreateInventoryItemUI;
        }

        private void Update()
        {
            RefreshAndDisplayScannedCollectables();
        }

        private void OnDestroy()
        {
            // m_OnInventoryItemAddedEvent.OnEventRaised -= CreateInventoryItemUI;
        }
        private void RefreshAndDisplayScannedCollectables()
        {
            if (!m_OnToggleInventoryUIEvent.Value)
            {
                return;
            }

            // Clear all the ItemUIs
            foreach (ItemUI itemUI in m_ItemUIs)
            {
                itemUI.ResetItemDataAndHide();
            }

            for (int i = 0, count = m_ItemUserHand.CollectablesScanned.Count; i < count; i++)
            {
                InventoryItem item = m_ItemUserHand.CollectablesScanned[i] as InventoryItem;
                if (item == null)
                    continue;

                if (m_ItemUIs.Count < count)
                {
                    /// If the ItemUI is not present for this index, then create a new instance of ItemUI
                    ItemUI itemUI = m_PoolManager.GetObjectFromPool(m_ItemUIPrefab.Name) as ItemUI;
                    itemUI.SetItemDataAndShow(item, this);
                    itemUI.transform.SetParent(m_ViscinityContentGO.transform);
                    m_ItemUIs.Add(itemUI);
                }
                else
                {   // If the ItemUI is present for this index, then just update the data
                    m_ItemUIs[i].SetItemDataAndShow(item, this);
                }
            }
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
            Instantiate(m_ItemUIPrefab, m_ViscinityContentGO.transform).SetItemDataAndShow(item, this);
        }

        public void RemoveItemUIFromList(ItemUI itemUI)
        {
            m_ItemUIs.Remove(itemUI);
        }

        public void AddItemUIToList(ItemUI itemUI)
        {
            m_ItemUIs.Add(itemUI);
        }
    }
}