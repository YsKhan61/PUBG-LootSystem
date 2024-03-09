using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Helmets
    /// </summary>
    public class HelmetItem : InventoryItem
    {
        Inventory m_Inventory;
        ItemUserHand m_ItemUserHand;

        public bool TryStoreAndCollect(ItemUserHand hand)
        {
            if (!TryStore(hand))
                return false;

            if (!TryCollect(hand))
                return false;

            // holding is optional
            hand.TryHoldItemInHand(this as IHoldable);
            return true;
        }

        public override bool TryStore(ItemUserHand hand)
        {
            if (hand.Inventory.HelmetItem != null)
            {
                if (!hand.Inventory.HelmetItem.TryRemoveAndDrop())
                    return false;
            }

            if (!hand.Inventory.TryAddHelmetToInventory(this))
                return false;

            (ItemData as HelmetDataSO).OnHelmetItemAddedToInventory.RaiseEvent(this);
            m_Inventory = hand.Inventory;
            return true;
        }

        public override bool TryCollect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            m_RootGO.transform.SetParent(hand.HelmetHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_RootGO.transform.localRotation = Quaternion.identity;
            m_ItemUserHand = hand;
            Debug.Log(Name + " is collected");
            return true;
        }

        public bool TryRemoveAndDrop()
        {
            if (!TryRemove(m_ItemUserHand))
                return false;

            m_ItemUserHand.TryPutAwayItem(this);
            return Drop(m_ItemUserHand);
        }

        public override bool TryRemove(ItemUserHand hand)
        {
            if (m_Inventory == null)
            {
                Debug.LogError("Inventory not found!");
                return false;
            }

            if (!m_Inventory.TryRemoveHelmetFromInventory(this))
                return false;

            (ItemData as HelmetDataSO).OnHelmetItemRemovedFromInventory.RaiseEvent(this);
            return true;
        }

        public override bool Drop(ItemUserHand hand)
        {
            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = hand.transform.position + hand.transform.forward * 2f;
            m_RootGO.transform.SetParent(null);

            Debug.Log(Name + " is dropped");
            return true;
        }
    }

}
