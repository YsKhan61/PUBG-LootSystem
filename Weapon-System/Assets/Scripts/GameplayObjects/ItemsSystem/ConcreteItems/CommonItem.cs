using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The items that can be collected and stored in inventory, and later can be dropped as well.
    /// </summary>
    public class CommonItem : ItemBase, ICollectable, IStorable, IDropable, IUsable
    {
        public bool IsCollected { get; protected set; }

        [SerializeField]
        GameObject m_Graphics;

        [SerializeField]
        GameObject m_RootGO;

        Transform m_CollectorTransform;

        public virtual bool Collect(ICollector collector)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            HideGraphics();
            m_CollectorTransform = collector.Transform;
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

            m_RootGO.transform.position = m_CollectorTransform.position + m_CollectorTransform.forward * 2f;
            ShowGraphics();
            Debug.Log(Name + " is dropped");
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