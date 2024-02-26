using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The items that can be collected and stored in inventory, and later can be dropped as well.
    ///  These don't need to be equipped in hand to be used.
    ///  Left click on it's icon in inventory to use it.
    /// </summary>
    public class CommonItem : ItemBase, ICollectable, IStorable, IDropable, IP_Usable
    {
        public bool IsCollected { get; protected set; }

        [SerializeField]
        GameObject m_Graphics;

        [SerializeField]
        GameObject m_RootGO;

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

        public virtual bool StoreInInventory(Inventory inventory)
        {
            inventory.AddCommonItem(this);
            return true;       // in progress ....
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

        public virtual bool PrimaryUse()
        {
            Debug.Log("Using " + Name);
            return true;
        }

        void ShowGraphics()
        {
            m_Graphics.SetActive(true);
        }

        void HideGraphics()
        {
            m_Graphics.SetActive(false);
        }
    }
}