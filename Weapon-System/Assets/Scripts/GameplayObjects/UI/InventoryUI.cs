using System.Collections.Generic;
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

        [SerializeField, Tooltip("When Toggle Inventory UI input is performed, this event is invoked. It is used to scan the nearby items only when Loot UI is visible in screen")]
        BoolEventChannelSO m_OnToggleInventoryUIEvent;

        [SerializeField, Tooltip("When an Inventory item is added to the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemAddedToInventory;


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
        Inventory m_Inventory;

        public float CanvasScaleFactor => m_Canvas.scaleFactor;

        List<ItemUI> m_ViscinityItemUIs;


        private void Start()
        {
            m_ViscinityItemUIs = new List<ItemUI>();

            m_OnInventoryItemAddedToInventory.OnEventRaised += CreateItemUIInInventorySlot;
        }

        private void Update()
        {
            RefreshAndDisplayScannedCollectables();
        }

        private void OnDestroy()
        {
            m_OnInventoryItemAddedToInventory.OnEventRaised -= CreateItemUIInInventorySlot;
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
            ItemUI itemUI = PoolManager.Instance.GetObjectFromPool(m_ItemUIPrefab.Name) as ItemUI;
            itemUI.SetItemDataAndShow(item, this, SlotType.Vicinity);
            itemUI.transform.SetParent(m_ViscinityContentTransform.transform);
            m_ViscinityItemUIs.Add(itemUI);
        }

        public void CreateItemUIInInventorySlot(InventoryItem item)
        {
            ItemUI itemUI = PoolManager.Instance.GetObjectFromPool(m_ItemUIPrefab.Name) as ItemUI;
            itemUI.SetItemDataAndShow(item, this, SlotType.Inventory);
            itemUI.transform.SetParent(m_InventoryContentTransform.transform);
        }

        public void ReleaseItemUIToPool(ItemUI itemUI)
        {
            if (itemUI != null)
            {
                itemUI.ResetItemDataAndHide();
                PoolManager.Instance.ReleaseObjectToPool(itemUI);
            }
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
                if (!m_ItemUserHand.CollectablesScanned.Contains(itemUI.StoredItem))
                {
                    ReleaseItemUIToPool(itemUI);
                }
            }
        }

        bool IsItemAlreadyPresentInVicinitySlot(in InventoryItem item)
        {
            for (int i = 0, count = m_ViscinityItemUIs.Count; i < count; i++)
            {
                if (m_ViscinityItemUIs[i].StoredItem == item)
                {
                    return true;
                }
            }
            return false;
        }

        void OnItemUIDroppedFromViscinityToInventory(ItemUI itemUI)
        {
            // bool success = m_ItemUserHand.TryStoreAndCollectInventoryItem(itemUI.StoredItem);
            
            bool success = itemUI.StoredItem.TryStoreAndCollect(m_ItemUserHand);
            if (success)
            {
                ReleaseItemUIToPool(itemUI);
            }

            // NOTE - Here we could reposition the ItemUI from VicinitySlot to InventorySlot
            // But, we are not doing it, as when we pickup inventory item by pressing pickup input,
            // we dont have vicinity slot's ItemUI to reposition.
            // Hence we are creating a new ItemUI in InventorySlot after we recieve an event
            // that an item is added to inventory.
        }

        void OnItemUIDroppedFromInventoryToViscinity(ItemUI itemUI)
        {
            bool success = (itemUI.StoredItem).TryRemoveAndDrop();
            if (success)
            {
                ReleaseItemUIToPool(itemUI);
            }
        }
    }
}