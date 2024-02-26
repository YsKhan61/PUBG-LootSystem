
using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Any item in the game, must need to have a name and a reference to the ItemDataSO
    /// </summary>
    public interface IItemInfo
    {
        public string Name { get; }
        public ItemDataSO ItemData { get; }
    }

    /// <summary>
    /// Interface for the collectable items
    /// </summary>
    public interface ICollectable
    {
        public bool IsCollected { get; }
        /// <summary>
        /// Collect the item.
        /// If some items need to be attached to the collector's hand, then the collector's transform is also passed
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public bool Collect(ItemUserHand hand);
    }


    /// <summary>
    /// Interface for the droppable items
    /// </summary>
    public interface IDropable
    {
        public bool Drop();
    }


    /// <summary>
    /// Interface for the storable items
    /// eg: Ammo, Heals, Attachments, etc.
    /// </summary>
    public interface IStorable
    {
        /// <summary>
        /// Store the item in the inventory
        /// Each item can be stored in different places in inventory,
        /// hence we send the inventory to the item to store itself in the inventory
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public bool StoreInInventory(Inventory inventory);
    }


    /// <summary>
    /// Interface for the usable items - primary use of the item
    /// Fire, Punch, Throw, etc.
    /// </summary>
    public interface  IP_Usable
    {
        public bool PrimaryUse();
    }


    /// <summary>
    /// Interface for the usable items - secondary use of the item
    /// eg: ADS
    /// </summary>
    public interface IS_Usable
    {
        public bool SecondaryUse();
    }


    /// <summary>
    /// Interface for the items that can be held in hand
    /// 
    /// ------------REMARKS--------------------
    /// An item that can be hold by the hand, 
    /// must need to be put away also at some point of time.
    /// eg: Guns, Melee weapons, Throwable items, etc.
    /// </summary>
    public interface IHoldable
    {
        public bool IsInHand { get; }
        /// <summary>
        /// We pass the hand to hold the item in
        /// </summary>
        public bool Hold();
        /// <summary>
        /// This method is used to either put away the item in the inventory
        /// or drop it on the ground
        /// </summary>
        /// <returns></returns>
        public bool PutAway();
    }


    /// <summary>
    /// Interface for the weapon attachment items
    /// Considering that item can be attached as well as detached from the weapon
    /// eg: Muzzle attachments, Magazine attachments, Sight attachments, Foregrip attachments, Stock attachments etc.
    /// </summary>
    public interface IWeaponAttachment
    {
        /// <summary>
        /// This method is used to attach the attachment to the weapon
        /// </summary>
        /// <param name="gun">the gun on which the attachment will be attached</param>
        /// <returns></returns>
        public bool AttachToWeapon(ISightHolder sightHolder);
        public bool DetachFromWeapon();
    }


    /// <summary>
    /// Interface for the holder of the sight attachment.
    /// eg: Guns
    /// </summary>
    public interface ISightHolder
    {
        public ISightAttachment SightAttachment { get; }        // needed in the AttachToWeapon method of the ISightAttachment
        public Transform SightHolderTransform { get; }          // needed in the AttachToWeapon method of the ISightAttachment
        public void AttachSight(ISightAttachment sight);
        public void DetachSight();
    }


    /// <summary>
    /// Interface for the sight attachment items. 
    /// eg: Red dot sight, Holographic sight, 2x, 4x, 8x scopes etc.
    /// </summary>
    public interface  ISightAttachment : IWeaponAttachment
    {
        public bool AimDownSight();
    }
}