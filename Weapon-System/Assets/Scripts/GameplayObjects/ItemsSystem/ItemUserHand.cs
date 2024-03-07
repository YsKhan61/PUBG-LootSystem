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


        [SerializeField, Tooltip("When an Weapon item is added to the inventory to specific index, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent;

        [SerializeField, Tooltip("Before an Weaopn item is removed from the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent;

        [SerializeField, Tooltip("When an Weapon item is removed from the inventory from specific index, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnAfterWeaponItemRemovedFromWeaponInventoryFromSpecificIndexEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other in the inventory, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemSwappedInInventoryEvent;


        [SerializeField, Tooltip("When a backpack is added to the inventory, this event is invoked")]
        BackpackItemEventChannelSO m_OnBackpackItemAddedToInventory;

        [SerializeField, Tooltip("When a backpack is removed from the inventory, this event is invoked")]
        BackpackItemEventChannelSO m_OnBackpackItemRemovedFromInventory;


        [SerializeField, Tooltip("When a helmet is added to the inventory, this event is invoked")]
        HelmetItemEventChannelSO m_OnHelmetItemAddedToInventory;

        [SerializeField, Tooltip("When a helmet is removed from the inventory, this event is invoked")]
        HelmetItemEventChannelSO m_OnHelmetItemRemovedFromInventory;


        [SerializeField, Tooltip("When a vest is added to the inventory, this event is invoked")]
        VestItemEventChannelSO m_OnVestItemAddedToInventory;

        [SerializeField, Tooltip("When a vest is removed from the inventory, this event is invoked")]
        VestItemEventChannelSO m_OnVestItemRemovedFromInventory;



        [Space(10)]

        [SerializeField]
        Inventory m_Inventory;

        [SerializeField]
        LayerMask m_ItemLayer;

        [SerializeField, Tooltip("This radius will be used for the OverlapSphere that will detect the collectable items nearby")]
        float m_Radius = 1f;

        [SerializeField, Tooltip("The backpack will be made child to this transform")]
        Transform m_BackpackHolderTransform;
        public Transform BackpackHolderTransform => m_BackpackHolderTransform;

        [SerializeField, Tooltip("The helmet will be made child to this transform")]
        Transform m_HelmetHolderTransform;
        public Transform HelmetHolderTransform => m_HelmetHolderTransform;

        [SerializeField, Tooltip("The vest will be made child to this transform")]
        Transform m_VestHolderTransform;
        public Transform VestHolderTransform => m_VestHolderTransform;


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


        /// <summary>
        /// Any item that implements IStorable can be stored in the inventory
        /// If stored, then it will be collected
        /// If collected, then it will be held in hand if it is IHoldable
        /// </summary>
        /// <param name="storable"></param>
        /// <returns></returns>
        public bool TryStoreCollectAndHoldItem(IStorable storable)
        {
            if (!TryStore(storable))
                return false;

            if (!TryCollect(storable))
                return false;

            // for now it's optional if the item is holdable
            TryHoldItemInHand(storable as IHoldable);
            return true;
        }

        public bool TryCollect(ICollectable collectable)
        {
            return collectable.TryCollect(this);
        }

        public bool TryStore(IStorable storable)
        {
            return storable.TryStore(this);
        }
        
        public bool TryRemove(IStorable storable)
        {
            return storable.TryRemove(this);
        }



        public bool TryStoreAndCollectInventoryItem(InventoryItem item)
        {
            if (!TryStoreInventoryItemAndRaiseEvent(item))
                return false;

            return TryCollect(item);
        }

        public bool TryStoreInventoryItemAndRaiseEvent(InventoryItem item)
        {
            if (!m_Inventory.TryAddItemToInventory(item))
                return false;

            m_OnInventoryItemAddedToInventory.RaiseEvent(item);
            return true;
        }

        public bool TryRemoveAndDropInventoryItem(InventoryItem item)
        {
            if (!TryRemoveInventoryItem(item))
                return false;

            return TryPutAwayAndDropItem(item);        
        }

        public bool TryRemoveInventoryItem(InventoryItem item)
        {
            return m_Inventory.TryRemoveItemFromInventory(item);
            // No event needed yet, if needed later we raise the event here.
        }



        /// <summary>
        /// First try to store the weapon in the weapon storage.
        /// Then try to collect it.
        /// Then try to hold it in hand.
        /// </summary>
        /// <param name="weaponItem"></param>
        /// <returns>true if success, false otherwise</returns>
        public bool TryStoreCollectAndHoldWeapon(WeaponItem weaponItem)
        {
            if (!TryStoreWeapon(weaponItem))
                return false;

            if (!TryCollect(weaponItem))
                return false;

            TryHoldItemInHand(weaponItem);

            return true;
        }

        public bool TryStoreWeapon(WeaponItem weaponItem)
        {
            bool added = false;
            int index;

            bool emptyIndexFound = m_Inventory.TryGetEmptyWeaponStorageIndex(out index);
            if (emptyIndexFound)
            {
                // Store weapon in that index
                added = m_Inventory.AddWeaponItemToStorageIndex(weaponItem, index);
            }

            if (!added)
            {
                bool weaponInHandFound = m_Inventory.TryGetIndexOfWeaponInHand(out index);
                if (weaponInHandFound)
                {
                    bool isRemoved = TryRemovePutAwayAndDropWeapon(index);
                    if (!isRemoved)
                    {
                        return false;
                    }
                    // Store weapon in that index
                    added = m_Inventory.AddWeaponItemToStorageIndex(weaponItem, index);
                }
            }

            if (!added)
            {
                index = 0;
                // Remove weapon from first index and store the new weapon in that index
                bool isRemoved = TryRemovePutAwayAndDropWeapon(index);
                if (!isRemoved)
                {
                    return false;
                }

                added = m_Inventory.AddWeaponItemToStorageIndex(weaponItem, index);
            }

            if (!added)
            {
                return false;
            }

            m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent.RaiseEvent(weaponItem, index);

            return true;
        }

        public bool TryRemovePutAwayAndDropWeapon(in int index)
        {
            if (!TryGetWeaponItemFromWeaponInventory(index, out WeaponItem weaponItem))
                return false;

            if (!TryRemoveWeapon(weaponItem))
                return false;

            return TryPutAwayAndDropItem(weaponItem);
        }

        public bool TryRemoveWeapon(WeaponItem weaponItem)
        {
            if (!m_Inventory.TryGetIndexOfWeaponItem(weaponItem, out int index))
            {
                return false;
            }

            m_OnBeforeWeaponItemRemovedFromWeaponInventoryEvent.RaiseEvent(weaponItem, index);

            if (!m_Inventory.TryRemoveWeaponItemFromStorage(index))
                return false;

            m_OnAfterWeaponItemRemovedFromWeaponInventoryFromSpecificIndexEvent.RaiseEvent(weaponItem, index);
            return true;
        }

        public bool SwapWeaponsInWeaponStorage(int leftIndex, int rightIndex)
        {
            if (!m_Inventory.TrySwapWeaponItemsInWeaponStorage(leftIndex, rightIndex))
                return false;

            m_OnWeaponItemSwappedInInventoryEvent.RaiseEvent(leftIndex, rightIndex);
            return true;
        }

        public bool TryGetWeaponItemFromWeaponInventory(int index, out WeaponItem weaponItem)
        {
            return m_Inventory.TryGetWeaponItem(index, out weaponItem);
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
            if (!TryStoreBackpack(backpackItem))
                return false;

            return TryCollect(backpackItem);
        }

        public bool TryStoreBackpack(BackpackItem backpackItem)
        {
            if (m_Inventory.BackpackItem != null)
            {
                if (!TryRemoveAndDropBackpack(m_Inventory.BackpackItem))
                    return false;
            }

            if (!m_Inventory.TryAddBackpackToInventory(backpackItem))
                return false;

            m_OnBackpackItemAddedToInventory.RaiseEvent(backpackItem);

            return true;
        }

        public bool TryRemoveAndDropBackpack(BackpackItem backpackItem)
        {
            if (!TryRemoveBackpack(backpackItem))
                return false;

            return TryPutAwayAndDropItem(backpackItem);
        }

        public bool TryRemoveBackpack(BackpackItem backpackItem)
        {
            if (!m_Inventory.TryRemoveBackpackFromInventory(backpackItem))
                return false;

            m_OnBackpackItemRemovedFromInventory.RaiseEvent(backpackItem);

            return true;
        }



        public bool TryStoreAndCollectHelmet(HelmetItem helmetItem)
        {
            if (!TryStoreHelmet(helmetItem))
                return false;

            return TryCollect(helmetItem);
        }

        public bool TryStoreHelmet(HelmetItem helmetItem)
        {
            if (m_Inventory.HelmetItem != null)
            {
                if (!TryRemoveAndDropHelmet(m_Inventory.HelmetItem))
                    return false;
            }

            if (!m_Inventory.TryAddHelmetToInventory(helmetItem))
                return false;

            m_OnHelmetItemAddedToInventory.RaiseEvent(helmetItem);
            return true;
        }

        public bool TryRemoveAndDropHelmet(HelmetItem helmetItem)
        {
            if (!TryRemoveHelmet(helmetItem))
                return false;

            return TryPutAwayAndDropItem(helmetItem);
        }

        public bool TryRemoveHelmet(HelmetItem helmetItem)
        {
            if (!m_Inventory.TryRemoveHelmetFromInventory(helmetItem))
                return false;

            m_OnHelmetItemRemovedFromInventory.RaiseEvent(helmetItem);
            return true;
        }



        public bool TryStoreAndCollectVest(VestItem vestItem)
        {
            if (!TryStoreVest(vestItem))
                return false;

            return TryCollect(vestItem);
        }

        public bool TryStoreVest(VestItem vestItem)
        {
            if (m_Inventory.VestItem != null)
            {
                if (!TryRemoveAndDropVest(m_Inventory.VestItem))
                    return false;
            }

            if (!m_Inventory.TryAddVestToInventory(vestItem))
                return false;

            m_OnVestItemAddedToInventory.RaiseEvent(vestItem);
            return true;
        }

        public bool TryRemoveAndDropVest(VestItem vestItem)
        {
            if (!TryRemoveVest(vestItem))
                return false;

            return TryPutAwayAndDropItem(vestItem);
        }

        public bool TryRemoveVest(VestItem vestItem)
        {
            if (!m_Inventory.TryRemoveVestFromInventory(vestItem))
                return false;

            m_OnVestItemRemovedFromInventory.RaiseEvent(vestItem);
            return true;
        }



        private void OnPickupInputEvent(bool _)
        {
            if (m_CollectablesScanned.Count <= 0)
                return;

            // Items that are not IStorable, can be collected instantly.
            // Storable items will be stored in inventory first and then collected.
            // as if store fails, we don't want to collect the item.
            if (m_CollectablesScanned[0] is not IStorable storable)
            {
                TryCollect(m_CollectablesScanned[0]);
                return;
            }

            TryStoreCollectAndHoldItem(storable);
        }

        private void OnPrimaryWeaponSelect(bool _)
        {
            HoldWeapon(0);
        }

        private void OnSecondaryWeaponSelect(bool _)
        {
            HoldWeapon(1);
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

        private void HoldWeapon(int index)
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

        private bool TryPutAwayAndDropItem(IDroppable item)
        {
            if (item as IHoldable == ItemInHand)
            {
                TryPutAwayItem(true);
            }

            return item.Drop(this);
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
