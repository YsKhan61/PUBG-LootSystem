using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  This class represents the guns in the game.
    ///  This item is not stored in the Inventory's CommonItems list. Hence we don't need to inherit from CommonItem.
    ///  It is a carry item, which can be collected, stored in inventory separately, dropped and used.
    ///  This can be later inherited from WeaponItem or UsableItem etc (can contain throwables, melee weapons etc)
    ///  But this is GunItem and only two guns can be collected and stored in inventory.
    /// </summary>
    public class GunItem : ItemBase, ICollectable, IStorable, IDropable, IP_Usable, IS_Usable, IHoldable
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

        public bool IsInHand { get; protected set; }

        public ISightAttachment SightAttachment { get; protected set; }

        [SerializeField]
        GameObject m_RootGO;

        [SerializeField]
        GameObject m_Graphics;

        Transform m_CollectorTransform;

        public virtual bool Collect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            
            m_RootGO.transform.position = hand.Transform.position;
            m_RootGO.transform.forward = hand.Transform.forward;
            m_RootGO.transform.SetParent(hand.Transform);

            m_CollectorTransform = hand.Transform;

            IsInHand = false;
            Hide();

            Debug.Log(Name + " is collected");
            return true;
        }

        public virtual bool StoreInInventory(Inventory inventory)
        {
            inventory.AddGunToGunInventory(this);
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

            IsInHand = false;

            Show();

            Debug.Log(Name + " is dropped");
            return true;
        }

        public virtual bool PrimaryUse()
        {
            Shoot();
            return true;
        }

        public virtual bool SecondaryUse()
        {
            // If there is a sight attachment, then aim down sight through it
            if (SightAttachment != null)
            {
                return SightAttachment.AimDownSight();
            }

            // If there is no sight attachment, then aim down sight through iron sight
            Debug.Log(Name + " doing ads with iron sight!");
            return true;
        }

        public virtual bool Hold()
        {
            IsInHand = true;
            Show();
            return true;
        }

        public virtual bool PutAway()
        {
            IsInHand = false;
            Hide();
            return true;
        }

        public virtual bool Shoot()
        {
            Debug.Log(Name + " shooting....!");
            return true;
        }

        public virtual void Show()
        {
            m_Graphics.SetActive(true);
        }

        public virtual void Hide()
        {
            m_Graphics.SetActive(false);
        }
    }

}
