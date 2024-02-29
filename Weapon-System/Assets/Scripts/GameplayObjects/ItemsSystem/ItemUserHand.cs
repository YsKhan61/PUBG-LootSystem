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

        [SerializeField, Tooltip("Listen to this event to remove the respective common item from inventory.")]
        InventoryItemEventChannelSO m_OnCommonItemUIRemovedEvent;

        [SerializeField, Tooltip("When an WeaponItemUI is removed from WeaponInventoryUI, this event is invoked after that")]
        WeaponItemIntEventChannelSO m_OnAfterWeaponItemUIRemovedEvent;

        [SerializeField, Tooltip("hen two WeaponItemUI's are swapped with each other, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemUISwappedEvent;

        [Space(10)]



        [Header("Broadcast to")]

        [SerializeField, Tooltip("When a common item is added to the inventory, this event is invoked.")]
        InventoryItemEventChannelSO m_InventoryItemAddedEvent;

        [SerializeField, Tooltip("When a gun item is added to the inventory, this event is invoked.")]
        WeaponItemIntEventChannelSO m_OnGunItemAddedEvent;



        [Space(10)]

        [SerializeField]
        Inventory m_Inventory;
        public Inventory Inventory => m_Inventory;

        [SerializeField]
        LayerMask m_ItemLayer;

        [SerializeField, Tooltip("This radius will be used for the OverlapSphere that will detect the collectable items nearby")]
        float m_Radius = 1f;

        public Transform Transform => transform;

        /// <remarks>
        /// For now we use GunItem, later we can use a base class for all items
        /// We need to have IHoldable and IUsable interfaces for the items
        /// </remarks>
        public WeaponItem ItemInHand { get; private set; }

        Collider[] resultColliders = new Collider[10];
        List<ICollectable> m_CollectablesScanned;

        private void Start()
        {
            m_CollectablesScanned = new List<ICollectable>();

            m_PickupInputEvent.OnEventRaised += CollectItem;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised += OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised += OnSecondaryWeaponSelect;
            m_HolsterItemInputEvent.OnEventRaised += TryPutAwayItem;
            m_OnCommonItemUIRemovedEvent.OnEventRaised += OnCommonItemUIRemovedEvent;
            m_OnAfterWeaponItemUIRemovedEvent.OnEventRaised += OnGunItemUIRemovedEvent;
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

            CheckForNearbyItems();

            // DisplayAllCollectables();            - Debug purposes
        }

        private void OnDestroy()
        {
            m_PickupInputEvent.OnEventRaised -= CollectItem;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised -= OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised -= OnSecondaryWeaponSelect;
            m_HolsterItemInputEvent.OnEventRaised -= TryPutAwayItem;
            m_OnCommonItemUIRemovedEvent.OnEventRaised -= OnCommonItemUIRemovedEvent;
            m_OnAfterWeaponItemUIRemovedEvent.OnEventRaised -= OnGunItemUIRemovedEvent;
        }

        private void CheckForNearbyItems()
        {
            m_CollectablesScanned.Clear();

            // Do an overlap Sphere to detect items
            int overlaps = Physics.OverlapSphereNonAlloc(transform.position, m_Radius, resultColliders, m_ItemLayer, QueryTriggerInteraction.Collide);
            if (overlaps > 0)
            {
                for (int i = 0; i < overlaps; i++)
                {
                    if (resultColliders[i].TryGetComponent(out ICollectable collectable))
                    {
                        if (collectable.IsCollected)
                        {
                            continue;
                        }
                        // Add to list of collectables
                        m_CollectablesScanned.Add(collectable);
                    }
                }
            }
        }

        private void CollectItem(bool _)
        {
            if (m_CollectablesScanned.Count <= 0)
                return;

            for (int i = 0; i < m_CollectablesScanned.Count; i++)
            {
                if (!m_CollectablesScanned[i].Collect(this))
                {
                    continue;
                }

                // Add 1st item to inventory
                TryStoreCollectableInInventory(m_CollectablesScanned[i]);
                TryHoldCollectableInHand(m_CollectablesScanned[i]);
                return;
            }
        }

        void TryStoreCollectableInInventory(ICollectable item)
        {
            // Each item can be stored in different places in the inventory,
            // hence we send the inventory to the item to store itself in the inventory
            // eg: Gun item, Armor, Ammo, etc.
            if (item is InventoryItem inventoryItem)
            {
                m_Inventory.AddItemToInventory(inventoryItem);
                m_InventoryItemAddedEvent.RaiseEvent(inventoryItem);
            }
            else if (item is WeaponItem gunItem)
            {
                if (!m_Inventory.TryAddWeaponItemToWeaponInventory(gunItem, out int storedIndex))
                    return;
                m_OnGunItemAddedEvent.RaiseEvent(gunItem, storedIndex);
            }
        }

        void TryHoldCollectableInHand(ICollectable item)
        {
            if (ItemInHand != null)
                return;

            ItemInHand = item as WeaponItem;
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
            WeaponItem primaryWeapon = m_Inventory.GetWeaponItem(0);

            if (primaryWeapon == null)
                return;

            if (ItemInHand == null)
            {
                ItemInHand = primaryWeapon;
            }
            ItemInHand.PutAway();
            ItemInHand = primaryWeapon;
            ItemInHand?.Hold();
        }

        private void OnSecondaryWeaponSelect(bool _)
        {
            WeaponItem secondaryWeapon = m_Inventory.GetWeaponItem(1);

            if (secondaryWeapon == null)
                return;

            if (ItemInHand == null)
            {
                ItemInHand = secondaryWeapon;
            }
            ItemInHand.PutAway();
            ItemInHand = secondaryWeapon;
            ItemInHand?.Hold();
        }

        private void OnCommonItemUIRemovedEvent(InventoryItem item)
        {
            item.Drop();
        }

        /// <summary>
        /// When the WeaponItemUI is removed from WeaponInventoryUI,
        /// Drop the item,
        /// if it was in hand, remove it from hand as well
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_"></param>
        private void OnGunItemUIRemovedEvent(WeaponItem item, int _)
        {
            item.Drop();

            if (ItemInHand == item)
            {
                ItemInHand = null;
            }
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
