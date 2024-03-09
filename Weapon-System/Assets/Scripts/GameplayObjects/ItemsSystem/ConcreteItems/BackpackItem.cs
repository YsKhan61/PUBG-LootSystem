using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Backpacks
    /// </summary>
    public class BackpackItem : InventoryItem
    {

        public override bool TryStore(ItemUserHand hand)
        {
            if (hand.Inventory.BackpackItem != null)
            {
                if (!hand.Inventory.BackpackItem.TryRemoveAndDrop())
                    return false;
            }

            if (!hand.Inventory.TryAddBackpackItem(this))
                return false;

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
            m_RootGO.transform.SetParent(hand.BackpackHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_RootGO.transform.localRotation = Quaternion.identity;
            m_ItemUserHand = hand;
            Debug.Log(Name + " is collected");
            return true;
        }

        public override bool TryRemove()
        {
            if (m_Inventory == null)
            {
                Debug.LogError("Inventory not found!");
                return false;
            }

            if (!m_Inventory.TryRemoveBackpackItem(this))
                return false;

            return true;
        }

        public override bool Drop()
        {
            if (m_ItemUserHand == null)
            {
                Debug.LogError("ItemUserHand not found!");
                return false;
            }

            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = m_ItemUserHand.transform.position + m_ItemUserHand.transform.forward * 2f;
            m_RootGO.transform.SetParent(null);

            m_ItemUserHand = null;
            Debug.Log(Name + " is dropped");
            return true;
        }
    }

}
