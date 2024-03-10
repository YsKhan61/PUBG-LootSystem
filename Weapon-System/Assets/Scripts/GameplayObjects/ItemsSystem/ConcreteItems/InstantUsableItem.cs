using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The items that can be collected and used instantly.
    ///  eg. Backpacks, Costumes, etc.
    /// </summary>
    public class InstantUsableItem : ItemBase, ICollectable, IP_Usable
    {
        public bool IsCollected { get; protected set; }

        /// <summary>
        /// For now we just destroy the entire gameobject
        /// </summary>
        /// <returns></returns>
        public virtual bool TryCollect(ItemUserHand _)
        {
            PrimaryUseStarted();
            return true;
        }

        public virtual bool PrimaryUseStarted()
        {
            Debug.Log(Name + " used!");
            return true;
        }

        public virtual bool PrimaryUseCanceled()
        {
            return true;
        }
    }
}