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

        [SerializeField, Tooltip("When an Inventory item is added to the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemAddedToInventory;

        [SerializeField, Tooltip("When an Inventory item is removed from the inventory, this event is invoked")]
        InventoryItemEventChannelSO m_OnInventoryItemRemovedFromInventory;

        [SerializeField, Tooltip("When an Weapon item is added to the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToWeaponInventoryEvent;

        [SerializeField, Tooltip("When an Weapon item is removed from the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemRemovedFromWeaponInventoryEvent;



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

            m_OnInventoryItemAddedToInventory.OnEventRaised += OnInventoryitemAddedToInventory;
            m_OnInventoryItemRemovedFromInventory.OnEventRaised += OnInventoryItemRemovedFromInventory;
            m_OnWeaponItemAddedToWeaponInventoryEvent.OnEventRaised += OnWeaponItemAddedToWeaponInventoryEvent;
            m_OnWeaponItemRemovedFromWeaponInventoryEvent.OnEventRaised += OnWeaponItemRemovedFromWeaponInventoryEvent;

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
            m_OnInventoryItemAddedToInventory.OnEventRaised -= OnInventoryitemAddedToInventory;
            m_OnInventoryItemRemovedFromInventory.OnEventRaised -= OnInventoryItemRemovedFromInventory;
            m_OnWeaponItemAddedToWeaponInventoryEvent.OnEventRaised -= OnWeaponItemAddedToWeaponInventoryEvent;
            m_OnWeaponItemRemovedFromWeaponInventoryEvent.OnEventRaised -= OnWeaponItemRemovedFromWeaponInventoryEvent;

            m_PickupInputEvent.OnEventRaised -= OnPickupInputEvent;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised -= OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised -= OnSecondaryWeaponSelect;
            m_HolsterItemInputEvent.OnEventRaised -= TryPutAwayItem;
        }

        private void OnWeaponItemAddedToWeaponInventoryEvent(WeaponItem item, int index)
        {
            if (item is not ICollectable collectable)
            {
                Debug.LogError("This cannot happen!");
                return;
            }
            collectable.Collect(this);
            TryHoldItemInHand(item);
        }

        private void OnWeaponItemRemovedFromWeaponInventoryEvent(WeaponItem item, int index)
        {
            TryDropItem(item);
        }

        private void OnInventoryitemAddedToInventory(InventoryItem item)
        {
            if (item is not ICollectable collecable)
            {
                Debug.LogError("This cannot happen!");
                return;
            }
            collecable.Collect(this);
            TryHoldItemInHand(item as IHoldable);
        }

        private void OnInventoryItemRemovedFromInventory(InventoryItem item)
        {
            TryDropItem(item);
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

            for (int i = 0; i < m_CollectablesScanned.Count; i++)
            {
                if (!m_CollectablesScanned[i].Collect(this))
                {
                    continue;
                }

                TryStoreCollectableInInventory(m_CollectablesScanned[i] as IStorable);
                return;
            }
        }

        void TryStoreCollectableInInventory(IStorable item)
        {
            if (item == null)
                return;

            // Each item can be stored in different places in the inventory,
            // hence we send the inventory to the item to store itself in the inventory
            // eg: Gun item, Armor, Ammo, etc.
            if (item is WeaponItem weaponItem)
            {
                m_Inventory.AddWeaponItemToWeaponInventory(weaponItem);
            }

            else if (item is InventoryItem inventoryItem)
            {
                m_Inventory.AddItemToInventory(inventoryItem);
            }
        }

        void TryHoldItemInHand(IHoldable item)
        {
            if (ItemInHand != null)
                return;

            ItemInHand = item;
            ItemInHand?.Hold();
        }

        void TryPutAwayItem(bool _)
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

        private void TryDropItem(IDroppable item)
        {
            if (item as IHoldable == ItemInHand)
            {
                TryPutAwayItem(true);
            }

            item.Drop();
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
