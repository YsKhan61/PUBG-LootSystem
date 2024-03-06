using System.Collections.Generic;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Attached to player to collect items and add to inventory (if the item is storable)
    /// It uses Trigger Collider to detect items
    /// It can also use the item in hand
    /// </summary>
    public class ItemUserHand : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("Listen to this event to hold the primary weapon in hand.")]
        BoolEventChannelSO m_PrimaryWeaponSelectInputEvent;

        [SerializeField, Tooltip("Listen to this event to hold the secondary weapon in hand.")]
        BoolEventChannelSO m_SecondaryWeaponSelectInputEvent;

        [SerializeField, Tooltip("Listen to this event to put away the item in hand.")]
        BoolEventChannelSO m_HolsterItemInputEvent;

        [SerializeField, Tooltip("This event notifies the pickup input performed")]
        BoolEventChannelSO m_PickupInputEvent;

        [SerializeField, Tooltip("This event notifies the primary use input performing")]
        BoolEventChannelSO m_PrimaryUseInputEvent;

        [SerializeField, Tooltip("This event notifies the secondary use input performing")]
        BoolEventChannelSO m_SecondaryUseInputEvent;

        [Space(10)]

        [Header("Broadcast to")]

        [SerializeField, Tooltip("When an Inventory item is added to the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemAddedToInventory;

        [SerializeField, Tooltip("When a backpack is added to the inventory, this event is invoked")]
        BackpackItemEventChannelSO m_OnBackpackItemAddedToInventory;

        [SerializeField, Tooltip("When a backpack is removed from the inventory, this event is invoked")]
        BackpackItemEventChannelSO m_OnBackpackItemRemovedFromInventory;

        [SerializeField, Tooltip("When an Weapon item is added to the inventory to specific index, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent;

        [SerializeField, Tooltip("Before an Weaopn item is removed from the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent;

        [SerializeField, Tooltip("When an Weapon item is removed from the inventory from specific index, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnAfterWeaponItemRemovedFromWeaponInventoryFromSpecificIndexEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other in the inventory, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemSwappedInInventoryEvent;


        [Space(10)]

        [SerializeField]
        Inventory m_Inventory;

        [SerializeField]
        LayerMask m_ItemLayer;

        [SerializeField, Tooltip("This radius will be used for the OverlapSphere that will detect the collectable items nearby")]
        float m_Radius = 1f;

        public Transform Transform => transform;

        /// <remarks>
        /// For now we use GunItem, later we can use a base class for all items
        /// We need to have IHoldable and IUsable interfaces for the items
        /// </remarks>
        public IHoldable ItemInHand { get; private set; }

        Collider[] resultColliders = new Collider[10];
        List<ICollectable> m_CollectablesScanned;
        public IList<ICollectable> CollectablesScanned => m_CollectablesScanned;


        private void Start()
        {
            m_CollectablesScanned = new List<ICollectable>();

            // m_OnInventoryItemAddedToInventory.OnEventRaised += OnInventoryitemAddedToInventory;
            // m_OnInventoryItemRemovedFromInventory.OnEventRaised += OnInventoryItemRemovedFromInventory;
            
            m_PickupInputEvent.OnEventRaised += OnPickupInputEvent;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised += OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised += OnSecondaryWeaponSelect;
            m_HolsterItemInputEvent.OnEventRaised += TryPutAwayItem;

        }

        private void Update()
        {
            if (m_PrimaryUseInputEvent.Value)
            {
                ((IP_Usable)ItemInHand)?.PrimaryUse();
            }

            if (m_SecondaryUseInputEvent.Value)
            {
                ((IS_Usable)ItemInHand)?.SecondaryUse();
            }

            ScanNearbyItems();

            // DisplayAllCollectables();            - Debug purposes
        }

        private void OnDestroy()
        {
            // m_OnInventoryItemAddedToInventory.OnEventRaised -= OnInventoryitemAddedToInventory;
            // m_OnInventoryItemRemovedFromInventory.OnEventRaised -= OnInventoryItemRemovedFromInventory;
            
            m_PickupInputEvent.OnEventRaised -= OnPickupInputEvent;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised -= OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised -= OnSecondaryWeaponSelect;
            m_HolsterItemInputEvent.OnEventRaised -= TryPutAwayItem;
        }


        public bool TryCollectItem(ICollectable item)
        {
            return item.Collect(this);
        }

        public bool TryStoreAndCollectInventoryItem(InventoryItem item)
        {
            bool isStored = TryStoreInventoryItemAndRaiseEvent(item);

            if (!isStored)
            {
                return false;
            }

            return TryCollectItem(item);
        }

        public bool TryStoreInventoryItemAndRaiseEvent(InventoryItem item)
        {
            bool isStored = m_Inventory.TryAddItemToInventory(item);
            if (!isStored)
            {
                return false;
            }

            m_OnInventoryItemAddedToInventory.RaiseEvent(item);
            return true;
        }

        public bool TryRemoveAndDropInventoryItem(InventoryItem item)
        {
            bool isRemoved = TryRemoveInventoryItem(item);
            if (!isRemoved)
            {
                return false;
            }

            return TryDropItem(item);        
        }

        public bool TryRemoveInventoryItem(InventoryItem item)
        {
            return m_Inventory.TryRemoveItemFromInventory(item);
            // No event needed yet, if needed later we raise the event here.
        }

        /// <summary>
        /// Try to store the backpack in the inventory and collect it.
        /// Before trying to store, we check if there is already a backpack in the inventory.
        /// If yes, we try remove it first.
        /// </summary>
        /// <param name="backpackItem"></param>
        /// <returns></returns>
        public bool TryStoreAndCollectBackpack(BackpackItem backpackItem)
        {
            if (m_Inventory.BackpackItem != null)
            {
                bool isRemoved = TryRemoveAndDropBackpack(m_Inventory.BackpackItem);
                if (!isRemoved)
                {
                    return false;
                }
            }

            bool isStored = m_Inventory.TryAddBackpackToInventory(backpackItem);
            if (!isStored)
            {
                return false;
            }

            TryCollectItem(backpackItem);
            m_OnBackpackItemAddedToInventory.RaiseEvent(backpackItem);

            return true;
        }

        public bool TryRemoveAndDropBackpack(BackpackItem backpackItem)
        {
            bool isRemoved = m_Inventory.TryRemoveBackpackFromInventory(backpackItem);
            if (!isRemoved)
            {
                return false;
            }
            TryDropItem(backpackItem);
            m_OnBackpackItemRemovedFromInventory.RaiseEvent(backpackItem);

            return true;
        }


        public bool TryStoreAndCollectWeaponInWeaponStorage(WeaponItem wepaonItem)
        {
            bool added = false;
            int index;

            bool emptyIndexFound = m_Inventory.TryGetEmptyWeaponStorageIndex(out index);
            if (emptyIndexFound)
            {
                // Store weapon in that index
                added = m_Inventory.AddWeaponItemToStorageIndex(wepaonItem, index);
            }

            if (!added)
            {
                bool weaponInHandFound = m_Inventory.TryGetIndexOfWeaponInHand(out index);
                if (weaponInHandFound)
                {
                    bool isRemoved = TryRemoveWeaponFromWeaponStorageIndex(index);
                    if (!isRemoved)
                    {
                        return false;
                    }
                    // Store weapon in that index
                    added = m_Inventory.AddWeaponItemToStorageIndex(wepaonItem, index);
                }
            }

            if (!added)
            {
                index = 0;
                // Remove weapon from first index and store the new weapon in that index
                bool isRemoved = TryRemoveWeaponFromWeaponStorageIndex(index);
                if (!isRemoved)
                {
                    return false;
                }

                added = m_Inventory.AddWeaponItemToStorageIndex(wepaonItem, index);
            }
            
            if (!added)
            {
                return false;
            }

            TryCollectItem(wepaonItem);
            TryHoldItemInHand(wepaonItem);

            m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent.RaiseEvent(wepaonItem, index);

            return true;
        }

        public bool TryRemoveWeaponFromWeaponStorageIndex(in int index)
        {
            bool isFound = m_Inventory.TryGetWeaponItem(index, out WeaponItem weaponItem);
            if (!isFound)
            {
                return false;
            }

            m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent.RaiseEvent(weaponItem, index);

            bool isRemoved = m_Inventory.TryRemoveWeaponItemFromStorage(index);
            if (!isRemoved)
            {
                return false;
            }

            TryDropItem(weaponItem);

            m_OnAfterWeaponItemRemovedFromWeaponInventoryFromSpecificIndexEvent.RaiseEvent(weaponItem, index);
            return true;
        }

        public bool SwapWeaponsInWeaponStorage(int leftIndex, int rightIndex)
        {
            bool isSwapped = m_Inventory.TrySwapWeaponItemsInWeaponStorage(leftIndex, rightIndex);
            if (!isSwapped)
            {
                return false;
            }

            m_OnWeaponItemSwappedInInventoryEvent.RaiseEvent(leftIndex, rightIndex);
            return true;
        }

        public bool TryGetWeaponItemFromWeaponInventory(int index, out WeaponItem weaponItem)
        {
            return m_Inventory.TryGetWeaponItem(index, out weaponItem);
        }


        private void ScanNearbyItems()
        {
            m_CollectablesScanned.Clear();

            // Do an overlap Sphere to detect items
            int overlaps = Physics.OverlapSphereNonAlloc(transform.position, m_Radius, resultColliders, m_ItemLayer, QueryTriggerInteraction.Collide);
            if (overlaps <= 0)
                return;

            for (int i = 0; i < overlaps; i++)
            {
                if (!resultColliders[i].TryGetComponent(out ICollectable collectable))
                {
                    continue;
                }

                if (collectable.IsCollected)
                {
                    continue;
                }
                // Add to list of collectables
                m_CollectablesScanned.Add(collectable);
            }
        }

        private void OnPickupInputEvent(bool _)
        {
            if (m_CollectablesScanned.Count <= 0)
                return;

            if (m_CollectablesScanned[0] is not IStorable storable)
            {
                TryCollectItem(m_CollectablesScanned[0]);
                return;
            }

            switch (storable)
            { 
                case BackpackItem:
                    TryStoreAndCollectBackpack(storable as BackpackItem);
                    break;

                case WeaponItem:
                    TryStoreAndCollectWeaponInWeaponStorage(storable as WeaponItem);
                    break;

                case InventoryItem:
                    TryStoreAndCollectInventoryItem(storable as InventoryItem);
                    break;
            }
        }
        
        private void TryHoldItemInHand(IHoldable item)
        {
            if (ItemInHand != null)
                return;

            ItemInHand = item;
            ItemInHand?.Hold();
        }

        private void TryPutAwayItem(bool _)
        {
            if (ItemInHand == null)
                return;

            ItemInHand.PutAway();
            ItemInHand = null;
        }

        private void OnPrimaryWeaponSelect(bool _)
        {
            HoldWeaponInHand(0);
        }

        private void OnSecondaryWeaponSelect(bool _)
        {
            HoldWeaponInHand(1);
        }

        private void HoldWeaponInHand(int index)
        {
            if (!m_Inventory.TryGetWeaponItem(index, out WeaponItem primaryWeapon))
                return;

            if (ItemInHand != null)
            {
                // If there is already an item in hand, then put it away
                ItemInHand.PutAway();
            }

            ItemInHand = primaryWeapon;
            ItemInHand?.Hold();
        }

        private bool TryDropItem(IDroppable item)
        {
            if (item as IHoldable == ItemInHand)
            {
                TryPutAwayItem(true);
            }

            return item.Drop();
        }

        #region Debug purposes

        private void DisplayAllCollectables()
        {
            Debug.Log("Total collectables scanned: " + m_CollectablesScanned.Count);
            foreach (ICollectable collectable in m_CollectablesScanned)
            {
                Debug.Log(collectable);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_Radius);
        }

        #endregion
    }

}
