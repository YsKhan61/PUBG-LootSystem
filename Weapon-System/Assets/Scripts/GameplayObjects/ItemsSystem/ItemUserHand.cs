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
    public class ItemUserHand : MonoBehaviour, ICollector
    {
        [Header("Listens to")]
        [SerializeField, Tooltip("This event notifies the pickup input performed")]
        BoolEventChannelSO m_PickupInputEvent;

        [SerializeField, Tooltip("This event notifies the firing input performing")]
        BoolEventChannelSO m_FiringInputEvent;

        public Transform Transform => transform;
        public IUsable ItemInHand { get; set; }

        [Space(10)]

        [SerializeField]
        Inventory m_Inventory;

        [SerializeField]
        LayerMask m_ItemLayer;

        [SerializeField, Tooltip("This radius will be used for the OverlapSphere that will detect the collectable items nearby")]
        float m_Radius = 3f;

        Collider[] resultColliders = new Collider[10];
        List<ICollectable> m_CollectablesScanned;

        private void Start()
        {
            m_CollectablesScanned = new List<ICollectable>();
            m_PickupInputEvent.OnEventRaised += OnPickupItems;
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
            m_PickupInputEvent.OnEventRaised -= OnPickupItems;
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

        private void OnPickupItems(bool _)
        {
            if (m_CollectablesScanned.Count > 0)
            {
                for (int i = 0; i < m_CollectablesScanned.Count; i++)
                {
                    if (m_CollectablesScanned[i].Collect(this))
                    {
                        // Add 1st item to inventory
                        TryStoreCollectableInInventory(m_CollectablesScanned[i]);
                        return;
                    }
                }
            }
        }

        void TryStoreCollectableInInventory(ICollectable item)
        {
            IStorable storable = item as IStorable;
            storable?.StoreInInventory(m_Inventory);
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
