using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Backpacks
    /// </summary>
    public class BackpackItem : InventoryItem
    {
        public override bool Collect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            m_RootGO.transform.SetParent(hand.BackpackHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_handTransform = hand.Transform;
            Debug.Log(Name + " is collected");
            return true;
        }

        public override bool Drop()
        {
            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = m_handTransform.position + m_handTransform.forward * 2f;
            m_RootGO.transform.SetParent(null);

            Debug.Log(Name + " is dropped");
            return true;
        }
    }

}
