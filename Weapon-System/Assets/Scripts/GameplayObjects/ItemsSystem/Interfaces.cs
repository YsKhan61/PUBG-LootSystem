
using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
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
    /// </summary>
    public interface  IP_Usable
    {
        public bool PrimaryUse();
    }

    /// <summary>
    /// Interface for the usable items - secondary use of the item
    /// </summary>
    public interface IS_Usable
    {
        public bool SecondaryUse();
    }

    /// <summary>
    /// Interface for the items that can be held in hand
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
    /// Interface for the shooter items (weapons, guns etc)
    /// </summary>
    public interface IShooter
    {
        public bool Shoot();
    }
}