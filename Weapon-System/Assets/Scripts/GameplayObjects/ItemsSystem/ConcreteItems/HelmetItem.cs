using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Helmets
    /// </summary>
    public class HelmetItem : InventoryItem
    {
        public override bool Collect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            m_RootGO.transform.SetParent(hand.HelmetHolderTransform);
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
