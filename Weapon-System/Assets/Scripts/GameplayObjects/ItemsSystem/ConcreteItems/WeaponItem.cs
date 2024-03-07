using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  This class represents the weapons in the game.
    ///  This item is not stored in the Inventory's InventoryItem's list
    ///  It is a carry item, which can be collected, stored in inventory separately, dropped and used.
    ///  This can be later inherited or extended to guns, launchers, throwables, melee weapons etc
    ///  ---------------------- NOIE----------------------
    ///  This Gun item can attach a sight attachment to it.
    /// </summary>
    public class WeaponItem : InventoryItem, IP_Usable, IS_Usable, IHoldable
    {
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
        public bool IsInHand { get; protected set; }
        public SightAttachmentItem SightAttachment { get; set; }
        public GripAttachmentItem GripAttachment { get; set; }
        public MuzzleAttachmentItem MuzzleAttachment { get; set; }


        public override bool TryCollect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            
            m_RootGO.transform.position = hand.Transform.position;
            m_RootGO.transform.forward = hand.Transform.forward;
            m_RootGO.transform.SetParent(hand.Transform);

            IsInHand = false;
            HideGraphics();

            Debug.Log(Name + " is collected");
            return true;
        }

        public override bool TryStore(ItemUserHand hand)
        {
            return hand.TryStoreWeapon(this);
        }

        public override bool TryRemove(ItemUserHand hand)
        {
            return hand.TryRemoveWeapon(this);
        }

        public override bool Drop(ItemUserHand hand)
        {
            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = hand.transform.position + hand.transform.forward * 2f;
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
    }

}
