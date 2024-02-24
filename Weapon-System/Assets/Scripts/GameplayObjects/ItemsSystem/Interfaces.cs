
using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public interface IPickable
    {
        public bool TryPick();
    }

    public interface IDropable
    {
        public bool TryDrop(Vector3 location);
    }

    public interface IStorable
    {
        public bool GetStoredInInventory(Inventory inventory);
    }

}