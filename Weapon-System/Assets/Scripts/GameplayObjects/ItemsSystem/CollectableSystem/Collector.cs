using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Attached to player to collect items and add to inventory
    /// It uses Trigger Collider to detect items
    /// </summary>
    public class Collector : MonoBehaviour
    {
        [Header("Listens to")]
        [SerializeField]
        BoolEventChannelSO m_PickupItemEvent;

        [Space(10)]

        [SerializeField]
        LayerMask m_ItemLayer;

        [SerializeField, Tooltip("This radius will be used for the OverlapSphere that will detect the collectable items nearby")]
        float m_Radius = 3f;

        [SerializeField]
        Inventory m_Inventory;

        Collider[] resultColliders = new Collider[10];
        List<CollectableBase> m_CollectablesScanned;

        private void Start()
        {
            m_CollectablesScanned = new List<CollectableBase>();
            m_PickupItemEvent.OnEventRaised += OnPickupItems;
        }

        private void OnDestroy()
        {
            m_PickupItemEvent.OnEventRaised -= OnPickupItems;
        }

        private void OnPickupItems(bool _)
        {
            if (m_CollectablesScanned.Count > 0)
            {
                // Add to inventory
                foreach (CollectableBase collectable in m_CollectablesScanned)
                {
                    if (collectable.TryPick())
                    {
                        TryStoreCollectableInInventory(collectable);
                        Debug.Log("Picked up " + collectable.Name);
                    }
                }
            }
        }

        private void Update()
        {
            m_CollectablesScanned.Clear();

            // Do an overlap Sphere to detect items
            int overlaps = Physics.OverlapSphereNonAlloc(transform.position, m_Radius, resultColliders, m_ItemLayer, QueryTriggerInteraction.Collide);
            if (overlaps > 0)
            {
                for (int i = 0; i < overlaps; i++)
                {
                    if (resultColliders[i].TryGetComponent(out CollectableBase collectable))
                    {
                        // Add to list of collectables
                        m_CollectablesScanned.Add(collectable);
                    }
                }
            }

            DisplayAllCollectables();
        }

        void TryStoreCollectableInInventory(CollectableBase collectableItem)
        {
            CollectableItem collectable = collectableItem as CollectableItem;
            if (collectable != null)
            {
                m_Inventory.AddItem(collectable.Item);
            }
        }

        private void DisplayAllCollectables()
        {
            foreach (CollectableBase collectable in m_CollectablesScanned)
            {
                Debug.Log(collectable.Name);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_Radius);
        }
    }

}
