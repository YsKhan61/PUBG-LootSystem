using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The base class for all collectables in the game
    ///  These items are game objects and exist in the game world.
    ///  These can be either collected and stored, later can be dropped, or used directly (no need to store or drop).
    ///  It can have graphics or not.
    /// </summary>
    public abstract class CollectableBase : MonoBehaviour, IPickable
    {
        public bool IsCollected { get; protected set; }

        public abstract string Name { get; }

        public abstract bool TryPick();
    }
}