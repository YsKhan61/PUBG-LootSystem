using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  This class represents the guns in the game.
    ///  This item is not stored in the Inventory's CommonItems list. Hence we don't need to inherit from CommonItem.
    ///  It is a carry item, which can be collected, stored in inventory separately, dropped and used.
    ///  This can be later inherited from WeaponItem or UsableItem etc (can contain throwables, melee weapons etc)
    ///  But this is GunItem and only two guns can be collected and stored in inventory.
    ///  ---------------------- NOIE----------------------
    ///  This Gun item can attach a sight attachment to it.
    /// </summary>
    public class WeaponItem : ItemBase, ICollectable, IStorable, IDroppable, IP_Usable, IS_Usable, IHoldable
    {
        [SerializeField, Tooltip("The root game object of this item")]
        GameObject m_RootGO;


        [SerializeField, Tooltip("The graphics model of this gun")]
        GameObject m_Graphics;


        [SerializeField, Tooltip("The sight will become a child of this game object with same position")]
        Transform m_SightHolderTransform;
        public Transform SightHolderTransform => m_SightHolderTransform;


        [SerializeField, Tooltip("The grip will become a child of this game object with same position")]
        Transform m_GripHolderTransform;
        public Transform GripHolderTransform => m_GripHolderTransform;

        [SerializeField, Tooltip("The muzzle will become a child of this game object with same position")]
        Transform m_MuzzleHolderTransform;
        public Transform MuzzleHolderTransform => m_MuzzleHolderTransform; 


        public WeaponDataSO WeaponData => m_ItemData as WeaponDataSO;
        public bool IsCollected { get; protected set; }
        public bool IsInHand { get; protected set; }
        public SightAttachmentItem SightAttachment { get; set; }
        public GripAttachmentItem GripAttachment { get; set; }
        public MuzzleAttachmentItem MuzzleAttachment { get; set; }

        // The transform of the collector, who collected this item
        // It is saved to drop the item at the same position and rotation
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
            HideGraphics();

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

            m_RootGO.transform.position = m_CollectorTransform.position + m_CollectorTransform.forward * 2f;
            m_RootGO.transform.SetParent(null);

            IsInHand = false;

            ShowGraphics();

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
            Debug.Log(Name);
            // If there is a sight attachment, then aim down sight through it
            if (SightAttachment != null)
            {
                return SightAttachment.AimDownSight();
            }

            // If there is no sight attachment, then aim down sight through iron sight
            Debug.Log("Aiming down sight through iron sight with ADS Zoom value of " + WeaponData.ADSZoomValue);
            return true;
        }

        public virtual bool Hold()
        {
            IsInHand = true;
            ShowGraphics();
            return true;
        }

        public virtual bool PutAway()
        {
            IsInHand = false;
            HideGraphics();
            return true;
        }

        bool Shoot()
        {
            Debug.Log(Name + " shooting....!");
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
