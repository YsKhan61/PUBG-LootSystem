using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Backpacks
    /// </summary>
    public class BackpackItem : InventoryItem
    {
        public override bool TryCollect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            m_RootGO.transform.SetParent(hand.BackpackHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_RootGO.transform.localRotation = Quaternion.identity;
            Debug.Log(Name + " is collected");
            return true;
        }

        public override bool TryStore(ItemUserHand hand)
        {
            return hand.TryStoreBackpack(this);
        }

        public override bool TryRemove(ItemUserHand hand)
        {
            return hand.TryRemoveBackpack(this);
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
