using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The items that can be collected and used instantly.
    ///  eg. Backpacks, Costumes, etc.
    /// </summary>
    public class InstantUsableItem : ItemBase, ICollectable, IUsable
    {
        /// <summary>
        /// For now we just destroy the entire gameobject
        /// </summary>
        /// <returns></returns>
        public virtual bool Collect()
        {
            Use();
            return true;
        }

        public virtual bool Use()
        {
            Debug.Log(Name + " used!");
            return true;
        }
    }
}