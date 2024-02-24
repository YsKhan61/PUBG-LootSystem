using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The items that can be collected and stored in inventory, and later can be dropped as well.
    /// </summary>
    public class CommonItem : ItemBase, ICollectable, IStorable, IDropable, IUsable
    {
        public bool IsPickedUp { get; protected set; }

        [SerializeField]
        GameObject m_Graphics;

        public virtual bool Collect()
        {
            if (IsPickedUp)
            {
                return false;
            }

            IsPickedUp = true;
            HideGraphics();
            return true;
        }

        public virtual bool StoreInInventory(Inventory inventory)
        {
            inventory.AddCommonItem(this);
            return true;       // in progress ....
        }

        public virtual bool Drop(Vector3 location)
        {
            if (!IsPickedUp)
            {
                return false;
            }

            IsPickedUp = false;

            transform.position = location;
            ShowGraphics();
            
            return true;
        }

        public virtual bool Use()
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