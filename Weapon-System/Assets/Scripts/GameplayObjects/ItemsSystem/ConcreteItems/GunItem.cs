using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class GunItem : ItemBase, ICollectable, IStorable, IDropable, IUsable
    {
        /*public enum WeaponType
        {
            AR,
            DMR,
            SMG,
            SR,
            Shotgun,
            Handgun,
            Melee,
            Throwable,
            Misselaneous
        }

        [SerializeField, Tooltip("Type of the weapon")]
        WeaponType m_WeaponType;

        [SerializeField, Tooltip("Damage done by the weapon")]
        int m_Damage;

        [SerializeField, Tooltip("The range upto which this weapon can register damage")]
        int m_Range;

        [SerializeField, Tooltip("Rate of firing per second")]
        float m_FireRate;

        [SerializeField, Tooltip("Force produced by firing from this weapon")]
        float m_Force;*/

        public bool IsCollected { get; protected set; }

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
            
            m_RootGO.transform.position = collector.Transform.position;
            m_RootGO.transform.forward = collector.Transform.forward;
            m_RootGO.transform.SetParent(collector.Transform);

            m_CollectorTransform = collector.Transform;

            Debug.Log(Name + " is collected");
            return true;
        }

        public virtual bool StoreInInventory(Inventory inventory)
        {
            inventory.AddGunToGunSlot(this);
            return true;
        }

        public virtual bool Drop()
        {
            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = m_CollectorTransform.position + m_CollectorTransform.forward * 2f;
            m_RootGO.transform.SetParent(null);

            Debug.Log(Name + " is dropped");
            return true;
        }

        public virtual bool Use()
        {
            Shoot();
            return true;
        }

        public virtual bool Shoot()
        {
            Debug.Log(Name + " shooting....!");
            return true;
        }
    }

}
