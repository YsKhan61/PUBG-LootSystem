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

        [SerializeField]
        Inventory m_Inventory;
        public Inventory Inventory => m_Inventory;

        [SerializeField]
        AimCameraController m_AimCameraController;
        public AimCameraController AimCameraController => m_AimCameraController;

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

            m_PickupInputEvent.OnEventRaised += OnPickupInputEvent;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised += OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised += OnSecondaryWeaponSelect;
            m_HolsterItemInputEvent.OnEventRaised += OnHolsterItemInputEvent;

            m_PrimaryUseInputEvent.OnEventRaised += OnPrimaryUseInputEvent;
            m_SecondaryUseInputEvent.OnEventRaised += OnSecondaryUseInputEvent;
        }

        private void Update()
        {
            ScanNearbyItems();
        }

        private void OnDestroy()
        {
            m_PickupInputEvent.OnEventRaised -= OnPickupInputEvent;
            m_PrimaryWeaponSelectInputEvent.OnEventRaised -= OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised -= OnSecondaryWeaponSelect;
            m_HolsterItemInputEvent.OnEventRaised -= OnHolsterItemInputEvent;

            m_PrimaryUseInputEvent.OnEventRaised -= OnPrimaryUseInputEvent;
            m_SecondaryUseInputEvent.OnEventRaised -= OnSecondaryUseInputEvent;
        }


        public bool SwapWeaponsInWeaponStorage(int leftIndex, int rightIndex)
        {
            if (!m_Inventory.TrySwapWeaponItemsInWeaponStorage(leftIndex, rightIndex))
                return false;

            // m_OnWeaponItemSwappedInInventoryEvent.RaiseEvent(leftIndex, rightIndex);
            return true;
        }

        public bool TryGetWeaponItemFromWeaponInventory(int index, out WeaponItem weaponItem)
        {
            return m_Inventory.TryGetWeaponItem(index, out weaponItem);
        }


        /// <remarks>
        /// Don't call this method directly, this method will only be called from the IHoldable item
        /// </remarks>
        internal void TryHoldItemInHand(IHoldable item)
        {
            if (ItemInHand != null)
                return;

            ItemInHand = item;
            ItemInHand?.Hold();
        }

        /// <remarks>
        /// Don't call this method directly, this method will only be called from the IHoldable item
        /// </remarks>
        internal void TryPutAwayItem()
        {
            if (ItemInHand == null)
                return;

            ItemInHand = null;
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
                // TryCollect(m_CollectablesScanned[0]);
                m_CollectablesScanned[0].TryCollect(this);
                return;
            }

            storable.TryStoreAndCollect(this);      // Items that are IHoldable, will automatically be held if possible through this.
        }

        private void OnPrimaryWeaponSelect(bool _)
        {
            HoldWeapon(0);
        }

        private void OnSecondaryWeaponSelect(bool _)
        {
            HoldWeapon(1);
        }

        private void OnHolsterItemInputEvent(bool _)
        {
            ItemInHand?.TryPutAway();
        }

        private void OnPrimaryUseInputEvent(bool value)
        {
            if (ItemInHand == null)
                return;

            if (value)
            {
                ((IP_Usable)ItemInHand)?.PrimaryUseStarted();
            }
            else
            {
                ((IP_Usable)ItemInHand)?.PrimaryUseCanceled();
            }
        }

        private void OnSecondaryUseInputEvent(bool value)
        {
            if (ItemInHand == null)
                return;

            if (value)
            {
                ((IS_Usable)ItemInHand)?.SecondaryUseStarted();
            }
            else
            {
                ((IS_Usable)ItemInHand)?.SecondaryUseCanceled();
            }
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
        
        private void HoldWeapon(int index)
        {
            if (!m_Inventory.TryGetWeaponItem(index, out WeaponItem primaryWeapon))
                return;

            if (ItemInHand != null)
            {
                // If there is already an item in hand, then put it away
                ItemInHand.TryPutAway();
            }

            ItemInHand = primaryWeapon;
            ItemInHand?.Hold();
        }


        #region Debug purposes

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_Radius);
        }

        #endregion
    }

}
