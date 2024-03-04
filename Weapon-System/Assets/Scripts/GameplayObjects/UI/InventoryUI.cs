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
        InventoryItemEventChannelSO m_OnInventoryItemAddedToInventory;

        [SerializeField, Tooltip("When an Inventory item is removed from the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemRemovedFromInventory;


        [Space(10)]

        [Header("Broadcast to")]

        [SerializeField, Tooltip("To add an weapon item in the inventory without any specific slot, this event is invoked")]
        WeaponItemEventChannelSO m_OnAddWeaponItemToWeaponInventoryEvent;

        [SerializeField, Tooltip("To add an inventory item in the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnAddInventoryItemToInventoryEvent;

        [SerializeField, Tooltip("To remove an inventory item from inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnRemoveInventoryItemFromInventoryEvent;



        [Space(10)]

        [SerializeField, Tooltip("The prefab that will be spawned as child of 'm_ContentGO' when a common item is stored in inventory")]
        ItemUI m_ItemUIPrefab;

        [SerializeField, Tooltip("To show ItemUI in Viscinity, The spawned instance of 'm_ItemUIPrefab' will be set as a child of this transform")]
        Transform m_ViscinityContentTransform;

        [SerializeField, Tooltip("To show ItemUI in Inventory, The spawned instance of 'm_ItemUIPrefab' will be set as a child of this transform")]
        Transform m_InventoryContentTransform;

        [SerializeField, Tooltip("The slots where various ItemUIs can be dropped.")]
        ItemSlotUI[] m_ItemSlotUIs;

        [SerializeField, Tooltip("The canvas of this whole UI")]
        Canvas m_Canvas;
        public Transform CanvasTransform => m_Canvas.transform;

        [SerializeField]
        ItemUserHand m_ItemUserHand;

        [SerializeField]
        PoolManager m_PoolManager;

        [SerializeField]
        Inventory m_Inventory;

        public float CanvasScaleFactor => m_Canvas.scaleFactor;

        List<ItemUI> m_ViscinityItemUIs;

        /// <summary>
        /// This is a temporary ItemUI that will be used to store the ItemUI 
        /// that is being dragged after it's item in it is stored in inventory
        /// </summary>
        ItemUI m_TempItemUI;


        private void Start()
        {
            m_ViscinityItemUIs = new List<ItemUI>();

            m_OnInventoryItemAddedToInventory.OnEventRaised += OnInventoryItemAddedToInventory;
            m_OnInventoryItemRemovedFromInventory.OnEventRaised += OnInventoryItemRemovedFromInventory;
        }

        private void Update()
        {
            RefreshAndDisplayScannedCollectables();
        }

        private void OnDestroy()
        {
            m_OnInventoryItemAddedToInventory.OnEventRaised -= OnInventoryItemAddedToInventory;
            m_OnInventoryItemRemovedFromInventory.OnEventRaised -= OnInventoryItemRemovedFromInventory;
        }

        

        /// <summary>
        /// When an ItemUI is dropped on a slot type of another ItemUI, or SlotUI
        /// </summary>
        /// <param name="droppedItemUI">The ItemUI that is being dropped</param>
        /// <param name="slotTypeOfOtherItemUI">The slot type of the ItemUI or SlotUI where the droppedItemUI is dropped.</param>
        public void OnItemUIDroppedOnSlotType(ItemUI droppedItemUI, SlotType slotTypeOfOtherItemUI)
        {
            // If the item is already in the same slot type, then return
            if (droppedItemUI.StoredSlotType == slotTypeOfOtherItemUI)
            {
                return;
            }

            switch (slotTypeOfOtherItemUI)
            {
                case SlotType.Inventory:
                    OnItemUIDroppedFromViscinityToInventory(droppedItemUI);
                    break;
                case SlotType.Vicinity:
                    OnItemUIDroppedFromInventoryToViscinity(droppedItemUI);
                    break;
            }
        }

        public void CreateItemUIForVicinitySlot(InventoryItem item)
        {
            ItemUI itemUI = m_PoolManager.GetObjectFromPool(m_ItemUIPrefab.Name) as ItemUI;
            itemUI.SetItemDataAndShow(item, this, SlotType.Vicinity);
            itemUI.transform.SetParent(m_ViscinityContentTransform.transform);
            m_ViscinityItemUIs.Add(itemUI);
        }

        public void CreateItemUIInInventorySlot(InventoryItem item)
        {
            ItemUI itemUI = m_PoolManager.GetObjectFromPool(m_ItemUIPrefab.Name) as ItemUI;
            itemUI.SetItemDataAndShow(item, this, SlotType.Inventory);
            itemUI.transform.SetParent(m_InventoryContentTransform.transform);
        }

        public void ReleaseItemUIToPool(ItemUI itemUI)
        {
            if (itemUI != null)
            {
                itemUI.ResetItemDataAndHide();
                m_PoolManager.ReleaseObjectToPool(itemUI);
            }
        }

        public void AddWeaponItemToWeaponInventory(WeaponItem weaponItem)
        {
            m_OnAddWeaponItemToWeaponInventoryEvent.RaiseEvent(weaponItem);
        }

        public bool TryCollectItem(InventoryItem item)
        {
            return m_ItemUserHand.TryCollectItem(item);
        }

        public void AddItemToInventory(InventoryItem item)
        {
            m_Inventory.TryAddItemToInventory(item);
        }

        private void RefreshAndDisplayScannedCollectables()
        {
            if (!m_OnToggleInventoryUIEvent.Value)
            {
                return;
            }

            // Create the ItemUIs for the items that are recently scanned and are not already present in the vicinity.
            for (int i = 0, count = m_ItemUserHand.CollectablesScanned.Count; i < count; i++)
            {
                InventoryItem item = m_ItemUserHand.CollectablesScanned[i] as InventoryItem;
                if (item == null)
                    continue;

                if (IsItemAlreadyPresentInVicinitySlot(item))
                {
                    continue;
                }

                CreateItemUIForVicinitySlot(item);
            }

            // Release the ItemUIs whose Item is not in the vicinity anymore.
            foreach (ItemUI itemUI in m_ViscinityItemUIs)
            {
                if (!m_ItemUserHand.CollectablesScanned.Contains(itemUI.Item))
                {
                    ReleaseItemUIToPool(itemUI);
                }
            }
        }

        bool IsItemAlreadyPresentInVicinitySlot(in InventoryItem item)
        {
            for (int i = 0, count = m_ViscinityItemUIs.Count; i < count; i++)
            {
                if (m_ViscinityItemUIs[i].Item == item)
                {
                    return true;
                }
            }
            return false;
        }

        void OnItemUIDroppedFromViscinityToInventory(ItemUI itemUI)
        {
            m_TempItemUI = itemUI;
            m_OnAddInventoryItemToInventoryEvent.RaiseEvent(itemUI.Item);
        }

        void OnItemUIDroppedFromInventoryToViscinity(ItemUI itemUI)
        {
            m_TempItemUI = itemUI;
            m_OnRemoveInventoryItemFromInventoryEvent.RaiseEvent(itemUI.Item);
        }

        private void OnInventoryItemAddedToInventory(InventoryItem item)
        {
            if (m_TempItemUI == null)
            {
                // THis happens when we press an input button to pick up item
                CreateItemUIInInventorySlot(item);
                return;
            }

            
            if (item != m_TempItemUI.Item)
            {
                Debug.LogError("This should never happen!");
                return;
            }

            // This happens when we drop an ItemUI from Viscinity to Inventory
            m_TempItemUI.transform.SetParent(m_InventoryContentTransform);
            m_TempItemUI.OnDragSuccess(SlotType.Inventory);
            m_ViscinityItemUIs.Remove(m_TempItemUI);
            m_TempItemUI = null;
        }

        private void OnInventoryItemRemovedFromInventory(InventoryItem item)
        {
            if (item != m_TempItemUI.Item)
            {
                Debug.LogError("This should never happen!");
                return;
            }

            // We destroy this ItemUI from InventoryUI, after it's item is already dropped.
            ReleaseItemUIToPool(m_TempItemUI);
        }
    }
}