using System.Collections;
using UnityEngine;
using Weapon_System.Utilities;


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
        [SerializeField]
        WeaponAnimator m_WeaponAnimator;
        public WeaponAnimator WeaponAnimator => m_WeaponAnimator;


        [Header("Attachment holders")]

        [SerializeField, Tooltip("Bullet will be spawned from this transform")]
        Transform m_BulletSpawnTransform;
        public Transform BulletSpawnTransform => m_BulletSpawnTransform;

        [SerializeField, Tooltip("The sight will become a child of this game object with same position")]
        Transform m_SightHolderTransform;
        public Transform SightHolderTransform => m_SightHolderTransform;

        [SerializeField, Tooltip("The grip will become a child of this game object with same position")]
        Transform m_GripHolderTransform;
        public Transform GripHolderTransform => m_GripHolderTransform;

        [SerializeField, Tooltip("The muzzle will become a child of this game object with same position")]
        Transform m_MuzzleHolderTransform;
        public Transform MuzzleHolderTransform => m_MuzzleHolderTransform;


        [Space(10)]

        [Header("Default Attachments")]

        [SerializeField, Tooltip("This is the iron sight or no sight")]
        SightAttachmentItem m_DefaultSight;

        public WeaponDataSO WeaponData => m_ItemData as WeaponDataSO;
        public bool IsInHand { get; protected set; }
        public bool IsAimingDownSight { get; protected set; }
        
        /// <summary>
        /// This is the sight attachment that is attached to the weapon. (not the iron sight)
        /// </summary>
        private SightAttachmentItem m_SightAttachment;
        public SightAttachmentItem SightAttachment 
        {
            get
            { 
                if (m_SightAttachment == null)
                {
                    // if no sight attachment is attached, then use the default sight
                    m_SightAttachment = m_DefaultSight;
                    m_SightAttachment.AttachToWeapon(this);
                }
                return m_SightAttachment;
            }
            set
            {
                m_SightAttachment = value;
            } 
        }
        public GripAttachmentItem GripAttachment { get; set; }
        public MuzzleAttachmentItem MuzzleAttachment { get; set; }

        Coroutine m_ShootRoutine;

        private WaitForSeconds m_FireRateWait;


        private void Start()
        {
            m_FireRateWait = new WaitForSeconds(1f / WeaponData.FireRate);
        }


        public override bool TryStoreAndCollect(ItemUserHand hand)
        {
            if (!TryStore(hand))
                return false;

            if (!TryCollect(hand))
                return false;

            // Holding is optional, only occurs for IHoldable items and when the hand is empty
            Hold();
            return true;
        }
        

        public override bool TryStore(ItemUserHand hand)
        {
            Inventory inventory = hand.Inventory;

            bool isAdded = false;

            bool emptyIndexFound = inventory.TryGetEmptyWeaponStorageIndex(out int index);
            if (emptyIndexFound)
            {
                // Store weapon in that index
                isAdded = inventory.TryAddWeaponItemToStorageIndex(this, index);
            }

            if (isAdded)
            {
                m_Inventory = inventory;
                return true;
            }

            WeaponItem weaponItem;

            if (inventory.TryGetIndexOfWeaponInHand(out index))
            {
                if (inventory.TryGetWeaponItem(index, out weaponItem))
                {
                    if (weaponItem.TryRemoveAndDrop())
                    {
                        // Store weapon in that index
                        isAdded = inventory.TryAddWeaponItemToStorageIndex(this, index);
                    }
                }
            }

            if (isAdded)
            {
                m_Inventory = inventory;
                return true;
            }

            index = 0;
            // Remove weapon from first index and store the new weapon in that index
            if (inventory.TryGetWeaponItem(index, out weaponItem))
            {
                if (weaponItem.TryRemoveAndDrop())
                {
                    // Store weapon in that index
                    isAdded = inventory.TryAddWeaponItemToStorageIndex(this, index);
                }
            }

            if (isAdded)
            {
                m_Inventory = inventory;
                return true;
            }

            return false;

            // m_OnWeaponItemAddedToWeaponInventoryToSpecificIndexEvent.RaiseEvent(weaponItem, index);
        }

        public override bool TryCollect(ItemUserHand hand)
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;

            m_RootGO.transform.position = hand.transform.position;
            m_RootGO.transform.forward = hand.transform.forward;
            m_RootGO.transform.SetParent(hand.transform);

            IsInHand = false;
            HideGraphics();

            m_ItemUserHand = hand;
            Debug.Log(Name + " is collected");
            return true;
        }

        public override bool TryRemoveAndDrop()
        {
            if (!TryRemove())
                return false;

            TryPutAway();
            return Drop();
        }

        public override bool TryRemove()
        {
            if (m_Inventory == null)
            {
                Debug.LogError("Inventory not found!");
                return false;
            }

            if (!m_Inventory.TryRemoveWeaponItem(this))
                return false;

            m_Inventory = null;
            return true;
        }

        public override bool Drop()
        {
            if (m_ItemUserHand == null)
            {
                Debug.LogError("ItemUserHand not found!");
                return false;
            }

            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            m_RootGO.transform.position = m_ItemUserHand.transform.position + m_ItemUserHand.transform.forward * 2f;
            m_RootGO.transform.SetParent(null);

            TryPutAway();

            ShowGraphics();

            m_ItemUserHand = null;
            Debug.Log(Name + " is dropped");
            return true;
        }

        public virtual bool PrimaryUseStarted()
        {
            StartShootRoutine();
            return true;
        }

        public virtual bool PrimaryUseCanceled()
        {
            StopShootRoutine();
            return true;
        }

        public virtual bool SecondaryUseStarted()
        {
            IsAimingDownSight = true;

            // Debug.Log(Name);
            // If there is a sight attachment, then aim down sight through it
            if (SightAttachment == null)
            {
                Debug.Log("It should not happen. No sight attachment found!");
                return false;
                
            }

            return SightAttachment.StartAimDownSight();
        }

        public virtual bool SecondaryUseCanceled()
        {
            IsAimingDownSight = false;

            if (SightAttachment == null)
            {
                Debug.Log("It should not happen. No sight attachment found!");
                return false;
            }

            return SightAttachment.StopAimDownSight();
        }

        public virtual bool Hold()
        {
            if (m_ItemUserHand == null)
            {
                Debug.LogError("ItemUserHand not found!");
                return false;
            }

            // If an item is already in hand, then put it away first
            m_ItemUserHand.ItemInHand?.TryPutAway();
            
            m_ItemUserHand.ItemInHand = this;
            IsInHand = true;
            
            ShowGraphics();

            return true;
        }

        public virtual bool TryPutAway()
        {
            if (m_ItemUserHand == null)
            {
                Debug.LogError("ItemUserHand not found!");
                return false;
            }

            if (m_ItemUserHand.ItemInHand != this as IHoldable)
                return false;

            m_ItemUserHand.ItemInHand = null;
            IsInHand = false;

            if (IsAimingDownSight)
            {
                SecondaryUseCanceled();
            }

            HideGraphics();
            return true;
        }

        void StartShootRoutine()
        {
            if (m_ShootRoutine != null)
            {
                StopCoroutine(m_ShootRoutine);
            }
            m_ShootRoutine = StartCoroutine(Shoot());
        }

        void StopShootRoutine()
        {
            if (m_ShootRoutine != null)
            {
                StopCoroutine(m_ShootRoutine);
            }
            m_ShootRoutine = null;
        }

        IEnumerator Shoot()
        {
            while (true)
            {
                if (TrySpawnBullet(out Bullet bullet))
                {
                    bullet.SetPositionAndRotation(BulletSpawnTransform.position, BulletSpawnTransform.rotation);
                    bullet.Fire();
                }

                yield return m_FireRateWait;
            }
        }

        bool TrySpawnBullet(out Bullet bullet)
        {
            bullet = null;
            // Spawn bullet
            ISpawnable spawnable = PoolManager.Instance.GetObjectFromPool(WeaponData.BulletPrefab.Name);
            if (spawnable == null)
            {
                Debug.LogError("Bullet not found in the pool");
                return false;
            }

            bullet = spawnable as Bullet;
            if (bullet == null)
            {
                Debug.LogError("Bullet not found in the pool");
                return false;
            }

            return true;
        }
    }

}
