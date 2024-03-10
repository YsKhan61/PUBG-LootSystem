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
    public class InventoryItem : ItemBase, IStorable
    {
        public bool IsCollected { get; protected set; }

        [SerializeField]
        GameObject m_Graphics;

        [SerializeField, Tooltip("The root game object of this item")]
        protected GameObject m_RootGO;

        protected Inventory m_Inventory;

        protected ItemUserHand m_ItemUserHand;
        public ItemUserHand ItemUserHand => m_ItemUserHand;


        public virtual bool TryStoreAndCollect(ItemUserHand hand)
        {
            if (!TryStore(hand))
                return false;

            if (!TryCollect(hand))
                return false;

            return true;
        }

        public virtual bool TryStore(ItemUserHand hand)
        {
            if (!hand.Inventory.TryAddInventoryItem(this))
                return false;

            m_Inventory = hand.Inventory;
            return true;
        }

        public virtual bool TryCollect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            HideGraphics();

            m_ItemUserHand = hand;
            Debug.Log(Name + " is collected");
            return true;
        }

        public virtual bool TryRemoveAndDrop()
        {
            if (!TryRemove())
                return false;

            return Drop();
        }

        public virtual bool TryRemove()
        {
            if (m_Inventory == null)
            {
                Debug.LogError("Inventory not found!");
                return false;
            }

            if (!m_Inventory.TryRemoveInventoryItem(this))
                return false;

            m_Inventory = null;
            return true;
        }

        public virtual bool Drop()
        {
            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = m_ItemUserHand.transform.position + m_ItemUserHand.transform.forward * 2f;
            ShowGraphics();

            m_ItemUserHand = null;
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