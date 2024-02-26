using System;
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

        [SerializeField, Tooltip("This event notifies the pickup input performed")]
        BoolEventChannelSO m_PickupInputEvent;

        [SerializeField, Tooltip("This event notifies the firing input performing")]
        BoolEventChannelSO m_FiringInputEvent;

        [SerializeField, Tooltip("Listen to this event to remove the respective common item from inventory.")]
        CommonItemEventChannelSO m_OnCommonItemUIRemovedEvent;

        [SerializeField, Tooltip("Listen to this event to remove the respective gun item from inventory.")]
        GunItemIntEventChannelSO m_OnGunItemUIRemovedEvent;

        [SerializeField, Tooltip("Listen to this event to swap the respective guns in inventory.")]
        IntIntEventChannelSO m_OnGunItemUISwappedEvent;

        public Transform Transform => transform;
        
        /// <remarks>
        /// For now we use GunItem, later we can use a base class for all items
        /// We need to have IHoldable and IUsable interfaces for the items
        /// </remarks>
        public GunItem ItemInHand { get; private set; }

        [Space(10)]

        [SerializeField]
        Inventory m_Inventory;
        public Inventory Inventory => m_Inventory;

        [SerializeField]
        LayerMask m_ItemLayer;

        [SerializeField, Tooltip("This radius will be used for the OverlapSphere that will detect the collectable items nearby")]
        float m_Radius = 3f;

        Collider[] resultColliders = new Collider[10];
        List<ICollectable> m_CollectablesScanned;

        private void Start()
        {
            m_CollectablesScanned = new List<ICollectable>();

            m_PickupInputEvent.OnEventRaised += CollectItem;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised += OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised += OnSecondaryWeaponSelect;
            m_OnCommonItemUIRemovedEvent.OnEventRaised += OnCommonItemUIRemovedEvent;
            m_OnGunItemUIRemovedEvent.OnEventRaised += OnGunItemUIRemovedEvent;
        }

        private void Update()
        {
            if (m_FiringInputEvent.Value)
            {
                ItemInHand?.Use();
            }

            CheckForNearbyItems();

            DisplayAllCollectables();
        }

        private void OnDestroy()
        {
            m_PickupInputEvent.OnEventRaised -= CollectItem;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised -= OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised -= OnSecondaryWeaponSelect;
            m_OnCommonItemUIRemovedEvent.OnEventRaised -= OnCommonItemUIRemovedEvent;
            m_OnGunItemUIRemovedEvent.OnEventRaised -= OnGunItemUIRemovedEvent;
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
            if (m_CollectablesScanned.Count > 0)
            {
                for (int i = 0; i < m_CollectablesScanned.Count; i++)
                {
                    if (m_CollectablesScanned[i].Collect(this))
                    {
                        // Add 1st item to inventory
                        TryStoreCollectableInInventory(m_CollectablesScanned[i]);
                        TryHoldCollectableInHand(m_CollectablesScanned[i]);
                        return;
                    }
                }
            }
        }

        void TryStoreCollectableInInventory(ICollectable item)
        {
            IStorable storable = item as IStorable;

            // Each item can be stored in different places in the inventory,
            // hence we send the inventory to the item to store itself in the inventory
            storable?.StoreInInventory(m_Inventory);
        }

        void TryHoldCollectableInHand(ICollectable item)
        {
            if (ItemInHand != null)
                return;

            ItemInHand = item as GunItem;
            ItemInHand?.Hold();
        }

        /// <summary>
        /// Called when HolsterWeaponInputEvent is raised
        /// </summary>
        void TryPutAwayItem()
        {
            if (ItemInHand == null)
                return;

            ItemInHand.PutAway();
            ItemInHand = null;
        }

        private void OnPrimaryWeaponSelect(bool _)
        {
            GunItem primaryWeapon = m_Inventory.GetPrimaryGun();
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
            GunItem secondaryWeapon = m_Inventory.GetSecondaryGun();
            if (ItemInHand == null)
            {
                ItemInHand = secondaryWeapon;
            }
            ItemInHand.PutAway();
            ItemInHand = secondaryWeapon;
            ItemInHand?.Hold();
        }

        private void OnCommonItemUIRemovedEvent(CommonItem item)
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
        private void OnGunItemUIRemovedEvent(GunItem item, int _)
        {
            item.Drop();

            if (ItemInHand == item)
            {
                ItemInHand = null;
            }
        }

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
    }

}
