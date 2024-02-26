using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The items that can be collected and stored in inventory, and later can be dropped as well.
    /// </summary>
    /// <remarks>
    ///  This is a general blueprint for the items that can be collected and stored in inventory.
    ///  If needed to be used, then it can be inherited and used as per the requirement.
    ///  There is no point of collecting and storing an item that cannot be used.
    ///  Example - A gift item that can be collected, and stored in the inventory, and passed to other friends, but cannot be used!
    ///
    ///  ------------------------ NOTE ------------------------
    ///  The inventory stores all items as a type of InventoryItem,
    ///  and then the item is casted to its actual type when used.
    ///  So, we have to make sure to inherit from InventoryItem, if we want to store the item in Inventory. 
    /// 
    /// </remarks>
    public class InventoryItem : ItemBase, ICollectable, IStorable, IDropable
    {
        public bool IsCollected { get; protected set; }

        [SerializeField]
        GameObject m_Graphics;

        [SerializeField, Tooltip("The root game object of this item")]
        protected GameObject m_RootGO;

        Transform m_handTransform;

        public virtual bool Collect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            HideGraphics();
            m_handTransform = hand.Transform;
            Debug.Log(Name + " is collected");
            return true;
        }

        public virtual bool StoreInInventory()
        {
            return true;
        }

        public virtual bool Drop()
        {
            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = m_handTransform.position + m_handTransform.forward * 2f;
            ShowGraphics();
            Debug.Log(Name + " is dropped");
            return true;
        }

        protected void ShowGraphics()
        {
            m_Graphics.SetActive(true);
        }

        protected void HideGraphics()
        {
            m_Graphics.SetActive(false);
        }
    }
}